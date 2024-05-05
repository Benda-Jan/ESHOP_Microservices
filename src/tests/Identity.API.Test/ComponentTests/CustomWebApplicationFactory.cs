
using Identity.Entities;
using Identity.Entities.DbSet;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.API.Test.ComponentTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public int UsersCount { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => 
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<UserContext>(options => 
            {
                options.UseInMemoryDatabase("Catalog-test-database");
            });
            
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserContext>();
            var encryptor = scope.ServiceProvider.GetRequiredService<IEncryptor>();

            var user1 = new User("User1", "User1@email.com", "SecretPassword123@", encryptor);
            context.Users.Add(user1);
            var user2 = new User("User2", "User2@email.com", "SecretPassword123@", encryptor);
            context.Users.Add(user2);
            context.SaveChanges();
        });

        builder.UseEnvironment("Development");
    }
}