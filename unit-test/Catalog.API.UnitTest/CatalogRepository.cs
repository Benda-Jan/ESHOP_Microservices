using Catalog.API.Commands;
using Catalog.API.EventsHandling;
using Catalog.API.Handlers;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.UnitTest;

public class UnitTest1
{
    [Fact]
    public async Task CreateItem_Correct()
    {
        var context = GetContext();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogItem = new CatalogItemInputDto()
        {
            Name = "TestingItem",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogTypeName = "Laptop",
            CatalogBrandName = "Apple",
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var sut = new CatalogRepository(context);
        var result = await sut.CreateItem(catalogItem);

        Assert.NotNull(result);
        var items = await context.CatalogItems.CountAsync();
        Assert.Equal(1, items);
        var exists = await context.CatalogItems.SingleOrDefaultAsync(x => x.Id == result.Id);
        Assert.NotNull(exists);
    }

    [Fact]
    public async Task CreateItem_AlreadyExists()
    {
        var context = GetContext();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogItem = new CatalogItemInputDto()
        {
            Name = "TestingItem",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogTypeName = "Laptop",
            CatalogBrandName = "Apple",
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var sut = new CatalogRepository(context);
        var result1 = await sut.CreateItem(catalogItem);

        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateItem(catalogItem));

        Assert.Contains("exists", ex.Message.ToLower());
    }

    [Fact]
    public async Task CreateItem_TypeNotExists()
    {
        var context = GetContext();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogItem = new CatalogItemInputDto()
        {
            Name = "AirPods",
            Description = "Description of AirPods",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogTypeName = "Headphones",
            CatalogBrandName = "Apple",
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var sut = new CatalogRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateItem(catalogItem));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task CreateItem_BrandNotExists()
    {
        var context = GetContext();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogItem = new CatalogItemInputDto()
        {
            Name = "Random laptop",
            Description = "Description of random laptop",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogTypeName = "Car",
            CatalogBrandName = "PackardBell",
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var sut = new CatalogRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateItem(catalogItem));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task RemoveItem_Correct()
    {
        var context = GetContext();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogType = await context.CatalogTypes.FirstOrDefaultAsync();
        var catalogBrand = await context.CatalogBrands.FirstOrDefaultAsync();

        var catalogItem = new CatalogItem()
        {
            Id = "1234",
            Name = "TestingItem",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogType,
            CatalogTypeId = catalogType!.Id,
            CatalogBrand = catalogBrand,
            CatalogBrandId = catalogBrand!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        await context.AddAsync(catalogItem);
        await context.SaveChangesAsync();

        var sut = new CatalogRepository(context);
        var result = await sut.RemoveItem(catalogItem.Id);

        Assert.NotNull(result);
        var items = await context.CatalogItems.CountAsync();
        Assert.Equal(0, items);
        var exists = await context.CatalogItems.SingleOrDefaultAsync(x => x.Id == result.Id);
        Assert.Null(exists);
    }



    private CatalogContext GetContext()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<CatalogContext>();
        builder.UseInMemoryDatabase("InMemoryForTesting").UseInternalServiceProvider(serviceProvider);

        var context = new CatalogContext(builder.Options);
        return context;
    }
}
