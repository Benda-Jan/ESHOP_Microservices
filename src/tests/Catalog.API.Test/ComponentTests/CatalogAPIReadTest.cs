
using System.Net.Http.Json;
using Catalog.Entities.DbSet;
using Catalog.API.Read;
using Catalog.Entities.Models;

namespace Catalog.API.Test.ComponentTests;

public class CatalogAPIReadTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly int _totalItems;
    private readonly int _totalBrands;
    private readonly int _totalTypes;

    public CatalogAPIReadTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _totalItems = factory.ItemsCount;
        _totalBrands = factory.BrandsCount;
        _totalTypes = factory.TypesCount;
    }

    [Theory]
    [InlineData("0", "10")]
    [InlineData("1", "10")]
    [InlineData("1", "5")]
    [InlineData("0", "5")]
    public async Task GetAllItems(string pageIndex, string pageSize)
    {
        var index = int.Parse(pageIndex);
        var size = int.Parse(pageSize);
        var response = await _client.GetAsync($"v1/Catalog/items?pageIndex={pageIndex}&pageSize={pageSize}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedItemsViewModel<CatalogItem>>();
        
        Assert.NotNull(result);
        Assert.Equal(index == 0 ? _totalItems : 0,result.ItemsOnPage.Count());
        Assert.Equal(index, result.PageIndex);
        Assert.Equal(size, result.PageSize);
        Assert.Equal(_totalItems, result.TotalItems);
    }

    [Theory]
    [InlineData("Apple")]
    [InlineData("Dell")]
    public async Task GetItemsWithBrand(string brandName)
    {
        var response = await _client.GetAsync($"v1/Catalog/items/brand/{brandName}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedItemsViewModel<CatalogItem>>();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.ItemsOnPage.Count());
        Assert.Equal(0, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(_totalItems, result.TotalItems);
    }

    [Theory]
    [InlineData("Laptop")]
    [InlineData("Phone")]
    public async Task GetItemsWithType(string typeName)
    {
        var response = await _client.GetAsync($"v1/Catalog/items/type/{typeName}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedItemsViewModel<CatalogItem>>();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.ItemsOnPage.Count());
        Assert.Equal(0, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(_totalItems, result.TotalItems);
    }

    [Theory]
    [InlineData("SpecificId1")]
    [InlineData("SpecificId2")]
    public async Task GetItemById_Correct(string id)
    {
        var response = await _client.GetAsync($"v1/Catalog/items/{id}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogItem?>();

        Assert.NotNull(result);
        Assert.Equal(id, result?.Id);
    }

    [Fact]
    public async Task GetItemById_NotExists()
    {
        var response = await _client.GetAsync($"v1/Catalog/items/{Guid.NewGuid().ToString()}");

        response.EnsureSuccessStatusCode();

        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task GetBrands()
    {
        var response = await _client.GetAsync($"v1/Catalog/brands");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogBrand[]>();
        
        Assert.NotNull(result);
        Assert.Equal(_totalBrands, result.Count());
    }

    [Fact]
    public async Task GetTypes()
    {
        var response = await _client.GetAsync($"v1/Catalog/types");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogBrand[]>();
        
        Assert.NotNull(result);
        Assert.Equal(_totalTypes, result.Count());
    }
}