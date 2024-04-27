
using Catalog.Entities.DbSet;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Catalog.API.Test.ComponentTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public int ItemsCount { get; private set; }
    public int BrandsCount { get; private set; }
    public int TypesCount { get; private set; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => 
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CatalogContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICacheService));
            if (cacheDescriptor != null)
                services.Remove(cacheDescriptor);

            services.AddDbContext<CatalogContext>(options => 
            {
                options.UseInMemoryDatabase("Catalog-test-database");
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
            
            services.AddTransient<ICacheService>(sp => {
                var options = Options.Create(new MemoryDistributedCacheOptions());
                IDistributedCache cache = new MemoryDistributedCache(options);
                return new RedisCacheService(cache);
            });

            var brandApple = new CatalogBrand() { Name = "Apple" };
            var brandDell = new CatalogBrand() { Name = "Dell" };
            var typeLaptop = new CatalogType() { Name = "Laptop" };
            var typePhone = new CatalogType() { Name = "Phone" };
            context.CatalogBrands.Add(brandApple);
            BrandsCount++;
            context.CatalogBrands.Add(brandDell);
            BrandsCount++;
            context.CatalogTypes.Add(typeLaptop);
            TypesCount++;
            context.CatalogTypes.Add(typePhone);
            TypesCount++;
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
            ItemsCount++;
            context.CatalogItems.Add(new CatalogItem()
            {
                Name = "Dell Vostro",
                Description = "Dell Vostro description",
                CatalogTypeId = typeLaptop.Id,
	            CatalogBrandId = brandDell.Id,
                CatalogType = typeLaptop,
                CatalogBrand = brandDell,
            });
            ItemsCount++;
            context.CatalogItems.Add(new CatalogItem()
            {
                Name = "IPhone 11 Pro",
                Description = "IPhone 11 Pro description",
                CatalogTypeId = typePhone.Id,
	            CatalogBrandId = brandApple.Id,
                CatalogType = typePhone,
                CatalogBrand = brandApple,
            });
            ItemsCount++;
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
            ItemsCount++;
            context.SaveChanges();
        });

        builder.UseEnvironment("Development");
    }
}