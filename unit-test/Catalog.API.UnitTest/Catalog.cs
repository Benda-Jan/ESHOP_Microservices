using Catalog.API.Write.Commands;
using Catalog.API.Write.EventsHandling;
using Catalog.API.Write.Handlers;
using Catalog.API.Read.Handlers;
using Catalog.Infrastructure.Services;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Catalog.API.UnitTest;

public class Catalog
{
    [Fact]
    public async Task CreateItem_Correct()
    {
        var context = GetContext();
        var cache = GetCache();
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

        var sut = new CatalogRepository(context, cache);
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
        var cache = GetCache();
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

        var sut = new CatalogRepository(context, cache);
        var result1 = await sut.CreateItem(catalogItem);

        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateItem(catalogItem));

        Assert.Contains("exists", ex.Message.ToLower());
    }

    [Fact]
    public async Task CreateItem_TypeNotExists()
    {
        var context = GetContext();
        var cache = GetCache();
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

        var sut = new CatalogRepository(context, cache);
        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateItem(catalogItem));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task CreateItem_BrandNotExists()
    {
        var context = GetContext();
        var cache = GetCache();
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

        var sut = new CatalogRepository(context, cache);
        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateItem(catalogItem));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task RemoveItem_Correct()
    {
        var context = GetContext();
        var cache = GetCache();
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

        var sut = new CatalogRepository(context, cache);
        var result = await sut.RemoveItem(catalogItem.Id);

        Assert.NotNull(result);
        var items = await context.CatalogItems.CountAsync();
        Assert.Equal(0, items);
        var exists = await context.CatalogItems.SingleOrDefaultAsync(x => x.Id == result.Id);
        Assert.Null(exists);
    }

    [Fact]
    public async Task GetAllItems_Correct()
    {
        var context = GetContext();
        var cache = GetCache();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogType = await context.CatalogTypes.FirstOrDefaultAsync();
        var catalogBrand = await context.CatalogBrands.FirstOrDefaultAsync();

        var catalogItem1 = new CatalogItem()
        {
            Id = "1234",
            Name = "TestingItem1",
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

        var catalogItem2 = new CatalogItem()
        {
            Id = "12345",
            Name = "TestingItem2",
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

        await context.AddAsync(catalogItem1);
        await context.AddAsync(catalogItem2);
        await context.SaveChangesAsync();

        var sut = new CatalogRepository(context, cache);
        var result = await sut.GetAllItems();

        Assert.NotNull(result.Items);
        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetAllItems_Correct_PageSize_PageIndex()
    {
        var context = GetContext();
        var cache = GetCache();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogType = await context.CatalogTypes.FirstOrDefaultAsync();
        var catalogBrand = await context.CatalogBrands.FirstOrDefaultAsync();

        var catalogItem1 = new CatalogItem()
        {
            Id = "1234",
            Name = "TestingItem1",
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

        var catalogItem2 = new CatalogItem()
        {
            Id = "12345",
            Name = "TestingItem2",
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

        await context.AddAsync(catalogItem1);
        await context.AddAsync(catalogItem2);
        await context.SaveChangesAsync();

        var sut = new CatalogRepository(context, cache);
        var result = await sut.GetAllItems(1, 1);

        Assert.NotNull(result.Items);
        Assert.Single(result.Items);
        Assert.Equal(catalogItem2, result.Items.First());
    }

    [Fact]
    public async Task GetItemsWithBrand_Correct()
    {
        var context = GetContext();
        var cache = GetCache();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogType = await context.CatalogTypes.FirstOrDefaultAsync();
        var catalogBrandApple = await context.CatalogBrands.FirstOrDefaultAsync(x => x.Name == "Apple");
        var catalogBrandDell = await context.CatalogBrands.FirstOrDefaultAsync(x => x.Name == "Dell");

        var catalogItem1 = new CatalogItem()
        {
            Id = "1234",
            Name = "TestingItem1",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogType,
            CatalogTypeId = catalogType!.Id,
            CatalogBrand = catalogBrandApple,
            CatalogBrandId = catalogBrandApple!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var catalogItem2 = new CatalogItem()
        {
            Id = "12345",
            Name = "TestingItem2",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogType,
            CatalogTypeId = catalogType!.Id,
            CatalogBrand = catalogBrandDell,
            CatalogBrandId = catalogBrandDell!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        await context.AddAsync(catalogItem1);
        await context.AddAsync(catalogItem2);
        await context.SaveChangesAsync();

        var sut = new CatalogRepository(context, cache);
        var result = await sut.GetItemsByBrand("Apple");

        Assert.NotNull(result.Items);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(catalogItem1, result.Items.First());
    }

    [Fact]
    public async Task GetItemsWithType_Correct()
    {
        var context = GetContext();
        var cache = GetCache();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogTypeLaptop = await context.CatalogTypes.FirstOrDefaultAsync(x => x.Name == "Laptop");
        var catalogTypePhone = await context.CatalogTypes.FirstOrDefaultAsync(x => x.Name == "Phone");
        var catalogBrand = await context.CatalogBrands.FirstOrDefaultAsync();

        var catalogItem1 = new CatalogItem()
        {
            Id = "1234",
            Name = "TestingItem1",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogTypePhone,
            CatalogTypeId = catalogTypePhone!.Id,
            CatalogBrand = catalogBrand,
            CatalogBrandId = catalogBrand!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var catalogItem2 = new CatalogItem()
        {
            Id = "12345",
            Name = "TestingItem2",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogTypeLaptop,
            CatalogTypeId = catalogTypeLaptop!.Id,
            CatalogBrand = catalogBrand,
            CatalogBrandId = catalogBrand!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        await context.AddAsync(catalogItem1);
        await context.AddAsync(catalogItem2);
        await context.SaveChangesAsync();

        var sut = new CatalogRepository(context, cache);
        var result = await sut.GetItemsByType("Phone");

        Assert.NotNull(result.Items);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(catalogItem1, result.Items.First());
    }

    [Fact]
    public async Task GetItemsId_Correct()
    {
        var context = GetContext();
        var cache = GetCache();
        var seeder = new CatalogContextSeeder(context);
        await seeder.SeedBrands();
        await seeder.SeedTypes();

        var catalogTypeLaptop = await context.CatalogTypes.FirstOrDefaultAsync(x => x.Name == "Laptop");
        var catalogTypePhone = await context.CatalogTypes.FirstOrDefaultAsync(x => x.Name == "Phone");
        var catalogBrand = await context.CatalogBrands.FirstOrDefaultAsync();

        var catalogItem1 = new CatalogItem()
        {
            Id = "1234",
            Name = "TestingItem1",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogTypePhone,
            CatalogTypeId = catalogTypePhone!.Id,
            CatalogBrand = catalogBrand,
            CatalogBrandId = catalogBrand!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        var catalogItem2 = new CatalogItem()
        {
            Id = "12345",
            Name = "TestingItem2",
            Description = "Description of TestingItem",
            Price = 123456,
            PictureFilename = string.Empty,
            PictureUri = string.Empty,
            CatalogType = catalogTypeLaptop,
            CatalogTypeId = catalogTypeLaptop!.Id,
            CatalogBrand = catalogBrand,
            CatalogBrandId = catalogBrand!.Id,
            AvailableStock = 50,
            MaxStockThreshold = 100,
            RestockThreshold = 10,
            OnReorder = false
        };

        await context.AddAsync(catalogItem1);
        await context.AddAsync(catalogItem2);
        await context.SaveChangesAsync();

        var sut = new CatalogRepository(context, cache);
        var result = await sut.GetItemById("1234");

        Assert.NotNull(result);
        Assert.Equal("1234", result.Id);
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

    private ICacheService GetCache()
    {
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(opts);

        return new RedisCacheService(cache);
    }
}
