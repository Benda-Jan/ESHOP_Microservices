using Catalog.API.Test.ComponentTests.MockedObjects;
using Catalog.API.Write.EventsHandling;
using Catalog.Entities.DbSet;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Catalog.API.Test.ComponentTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services => 
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CatalogContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICacheService));
            if (cacheDescriptor != null)
                services.Remove(cacheDescriptor);

            var rabbitMQRemovedDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(EventBusCatalogItemDeleted));
            if (rabbitMQRemovedDescriptor != null)
                services.Remove(rabbitMQRemovedDescriptor);

            var rabbitMQUpdatedDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(EventBusCatalogItemUpdated));
            if (rabbitMQUpdatedDescriptor != null)
                services.Remove(rabbitMQUpdatedDescriptor);

            var rabbitMQUpdatedConsumerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHostedService) && d.ImplementationFactory != null);
            if (rabbitMQUpdatedConsumerDescriptor != null)
                services.Remove(rabbitMQUpdatedConsumerDescriptor);

            services.AddDbContext<CatalogContext>(options => 
            {
                options.UseInMemoryDatabase("Catalog-test-database");
            });
            
            services.AddTransient<ICacheService>(sp => {
                var options = Options.Create(new MemoryDistributedCacheOptions());
                IDistributedCache cache = new MemoryDistributedCache(options);
                return new RedisCacheService(cache);
            });

            services.AddScoped<ItemUpdatedConsumer>(sp => 
            {
                var scope = sp.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ICatalogRepository>();
                return new FakeRabbitMQItemUpdatedConsumer(repository, new FakeRabbitMQPublisherDeleted());
            });

            services.AddScoped<EventBusCatalogItemDeleted>(sp => new FakeRabbitMQPublisherDeleted());
            services.AddScoped<EventBusCatalogItemUpdated>(sp => new FakeRabbitMQPublisherUpdated());

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
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            var brandApple = new CatalogBrand() { Name = "Apple" };
            var brandDell = new CatalogBrand() { Name = "Dell" };
            var typeLaptop = new CatalogType() { Name = "Laptop" };
            var typePhone = new CatalogType() { Name = "Phone" };
            context.CatalogBrands.Add(brandApple);
            context.CatalogBrands.Add(brandDell);
            context.CatalogTypes.Add(typeLaptop);
            context.CatalogTypes.Add(typePhone);
            if (!await context.CatalogItems.AnyAsync(x => x.Id == "SpecificId1"))
            {
                context.CatalogItems.Add(new CatalogItem()
                {
                    Id = "SpecificId1",
                    Name = "MacBook",
                    Description = "MacBook description",
                    CatalogTypeId = typeLaptop.Id,
                    CatalogBrandId = brandApple.Id,
                    CatalogType = typeLaptop,
                    CatalogBrand = brandApple,
                });
            }
            if (!await context.CatalogItems.AnyAsync(x => x.Name == "Dell Vostro"))
            {
                context.CatalogItems.Add(new CatalogItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Dell Vostro",
                    Description = "Dell Vostro description",
                    CatalogTypeId = typeLaptop.Id,
                    CatalogBrandId = brandDell.Id,
                    CatalogType = typeLaptop,
                    CatalogBrand = brandDell,
                });
            }
            if (!await context.CatalogItems.AnyAsync(x => x.Name == "IPhone 11 Pro"))
            {
                context.CatalogItems.Add(new CatalogItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "IPhone 11 Pro",
                    Description = "IPhone 11 Pro description",
                    CatalogTypeId = typePhone.Id,
                    CatalogBrandId = brandApple.Id,
                    CatalogType = typePhone,
                    CatalogBrand = brandApple,
                });
            }
            if (!await context.CatalogItems.AnyAsync(x => x.Id == "SpecificId2"))
            {
                context.CatalogItems.Add(new CatalogItem()
                {
                    Id = "SpecificId2",
                    Name = "Dell Galaxy A11",
                    Description = "Dell Galaxy A11 description",
                    CatalogTypeId = typePhone.Id,
                    CatalogBrandId = brandDell.Id,
                    CatalogType = typePhone,
                    CatalogBrand = brandDell,
                });
            }
            context.SaveChanges();
        });

        builder.UseEnvironment("Development");
    }
}