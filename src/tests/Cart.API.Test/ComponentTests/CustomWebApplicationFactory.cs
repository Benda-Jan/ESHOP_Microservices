using Cart.API.EventsHandling;
using Cart.API.Test.ComponentTests.MockedObjects;
using Cart.Entities.DbSet;
using Cart.Infrastructure;
using Cart.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Cart.API.Test.ComponentTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services => 
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CartContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var rabbitMQUpdatedDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(EventBusCartItemUpdated));
            if (rabbitMQUpdatedDescriptor != null)
                services.Remove(rabbitMQUpdatedDescriptor);

            var rabbitMQConsumerDescriptors = services.Where(d => d.ServiceType == typeof(IHostedService) && d.ImplementationFactory != null);
            while (!rabbitMQConsumerDescriptors.IsNullOrEmpty())
                services.Remove(rabbitMQConsumerDescriptors.First());

            var rabbitMQDeletedConsumerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHostedService) && d.ImplementationFactory != null);
            if (rabbitMQDeletedConsumerDescriptor != null)
                services.Remove(rabbitMQDeletedConsumerDescriptor);

            services.AddDbContext<CartContext>(options => 
            {
                options.UseInMemoryDatabase("Cart-test-database");
            });

            services.AddScoped<ItemUpdatedConsumer>(sp => 
            {
                var scope = sp.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
                return new FakeRabbitMQItemUpdatedConsumer(repository);
            });

            services.AddScoped<ItemDeletedConsumer>(sp => 
            {
                var scope = sp.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
                return new FakeRabbitMQItemDeletedConsumer(repository);
            });

            services.AddScoped<EventBusCartItemUpdated>(sp => new FakeRabbitMQPublisherUpdated());

            services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

            services.AddAuthorization(x =>
            {
                x.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("TestScheme")
                    .RequireAuthenticatedUser()
                    .Build();
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CartContext>();

            
            var cartItem1 = new CartItem
            {
                Id = "SpecificId1",
                CatalogItemId = "CatalogItemId1",
                UserId = "User1",
                Name = "Name1",
                Price = 123456,
                Quantity = 5,
            };
            var cartItem2 = new CartItem
            {
                Id = "SpecificId2",
                CatalogItemId = "CatalogItemId2",
                UserId = "User1",
                Name = "Name2",
                Price = 123456,
                Quantity = 5,
            };
            var cartItem3 = new CartItem
            {
                Id = "SpecificId3",
                CatalogItemId = "CatalogItemId3",
                UserId = "User2",
                Name = "Name3",
                Price = 123456,
                Quantity = 5,
            };
            var cartItem4 = new CartItem
            {
                Id = "SpecificId4",
                CatalogItemId = "CatalogItemId4",
                UserId = "User2",
                Name = "Name4",
                Price = 123456,
                Quantity = 5,
            };
            if (!await context.UserCarts.AnyAsync(x => x.UserId == "User1"))
            {
                context.UserCarts.Add(new UserCart() 
                { 
                    UserId = "User1",
                    CartItems = {cartItem1, cartItem2},
                });
            }
            if (!await context.UserCarts.AnyAsync(x => x.UserId == "User2"))
            {
                context.UserCarts.Add(new UserCart() 
                { 
                    UserId = "User2",
                    CartItems = {cartItem3, cartItem4},
                });
            }
            context.SaveChanges();
        });

        builder.UseEnvironment("Development");
    }
}