
using System.Net.Http.Json;
using Catalog.Entities.DbSet;
using Catalog.API.Write;
using Catalog.Entities.Dtos;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Catalog.API.Test.ComponentTests;

public class CatalogAPIWriteTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CatalogAPIWriteTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");
    }

    [Fact]
    public async Task CreateItem()
    {
        var input = new CatalogItemInputDto 
        {
            Name = "InputName",
            Description = "InputDescription",
            Price = 12345,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = "Laptop",
            CatalogBrandName = "Apple",
            AvailableStock = 100,
            RestockThreshold = 150,
            MaxStockThreshold = 1000,
            OnReorder = false,
        };
        var response = await _client.PostAsJsonAsync("v1/Catalog", input);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogItem>();
        
        Assert.NotNull(result);
        Assert.Equal(result.Name, input.Name);
        Assert.Equal(result.Description, input.Description);
        Assert.Equal(result.Price, input.Price);
        Assert.NotNull(result.CatalogType);
        Assert.Equal(result.CatalogType.Name, input.CatalogTypeName);
        Assert.NotNull(result.CatalogBrand);
        Assert.Equal(result.CatalogBrand.Name, input.CatalogBrandName);
        Assert.Equal(result.AvailableStock, input.AvailableStock);
        Assert.Equal(result.RestockThreshold, input.RestockThreshold);
        Assert.Equal(result.MaxStockThreshold, input.MaxStockThreshold);
        Assert.Equal(result.OnReorder, input.OnReorder);
    }

    [Theory]
    [InlineData("Dell Vostro", "Dell")]
    public async Task CreateItem_AlreadyExists(string name, string brand)
    {
        var input = new CatalogItemInputDto 
        {
            Name = name,
            Description = "InputDescription",
            Price = 12345,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = "Laptop",
            CatalogBrandName = brand,
            AvailableStock = 100,
            RestockThreshold = 150,
            MaxStockThreshold = 1000,
            OnReorder = false,
        };
        var response = await _client.PostAsJsonAsync("v1/Catalog", input);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Contains("already exists", result);
    }

    [Theory]
    [InlineData("Huawei")]
    [InlineData("Nokia")]
    public async Task CreateItem_BrandDoesNotExist(string brand)
    {
        var input = new CatalogItemInputDto 
        {
            Name = "RandomName",
            Description = "InputDescription",
            Price = 12345,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = "Laptop",
            CatalogBrandName = brand,
            AvailableStock = 100,
            RestockThreshold = 150,
            MaxStockThreshold = 1000,
            OnReorder = false,
        };
        var response = await _client.PostAsJsonAsync("v1/Catalog", input);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(Regex.Match(result, @"brand.*does not exist", RegexOptions.IgnoreCase).Success);
    }
    
    [Theory]
    [InlineData("Tablet")]
    [InlineData("PC")]
    public async Task CreateItem_TypeDoesNotExist(string type)
    {
        var input = new CatalogItemInputDto 
        {
            Name = "RandomName",
            Description = "InputDescription",
            Price = 12345,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = type,
            CatalogBrandName = "Apple",
            AvailableStock = 100,
            RestockThreshold = 150,
            MaxStockThreshold = 1000,
            OnReorder = false,
        };
        var response = await _client.PostAsJsonAsync("v1/Catalog", input);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(Regex.Match(result, @"type.*does not exist", RegexOptions.IgnoreCase).Success);
    }

    [Theory]
    [InlineData("LG")]
    [InlineData("Philips")]
    public async Task CreateBrand(string brandName)
    {
        var response = await _client.PostAsJsonAsync("v1/Catalog/brand", brandName);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogBrand>();
        
        Assert.NotNull(result);
        Assert.Equal(result.Name, brandName);
    }

    [Theory]
    [InlineData("Apple")]
    public async Task CreateBrand_AlreadyExists(string brandName)
    {
        var response = await _client.PostAsJsonAsync("v1/Catalog/brand", brandName);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(Regex.Match(result, @"brand.*already exists", RegexOptions.IgnoreCase).Success);
    }

    [Theory]
    [InlineData("Fridge")]
    [InlineData("Microwave")]
    public async Task CreateType(string typeName)
    {
        var response = await _client.PostAsJsonAsync("v1/Catalog/type", typeName);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogType>();
        
        Assert.NotNull(result);
        Assert.Equal(result.Name, typeName);
    }

    [Theory]
    [InlineData("Laptop")]
    [InlineData("Phone")]
    public async Task CreateType_AlreadyExists(string brandName)
    {
        var response = await _client.PostAsJsonAsync("v1/Catalog/type", brandName);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(Regex.Match(result, @"type.*already exists", RegexOptions.IgnoreCase).Success);
    }

    [Fact]
    public async Task UpdateItem()
    {
        var id = "SpecificId1";
        var newName = "RandomNewName";
        var newDescription = "RandomNewDescription";
        var newPrice = 654321;
        var newAvailableStock = 1000;
        var newRestockThreshold = 1000;
        var newMaxStockThreshold = 1000;
        var newOnRestock = true;
        var item = new CatalogItemInputDto
        {
            Name = newName,
            Description = newDescription,
            Price = newPrice,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = "Laptop",
            CatalogBrandName = "Apple",
            AvailableStock = newAvailableStock,
            RestockThreshold = newRestockThreshold,
            MaxStockThreshold = newMaxStockThreshold,
            OnReorder = newOnRestock,
        };
        var response = await _client.PutAsJsonAsync($"v1/Catalog?itemId={id}", item);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogItem?>();

        Assert.NotNull(result);
        Assert.Equal(result.Name, newName);
        Assert.Equal(result.Description, newDescription);
        Assert.Equal(result.Price, newPrice);
        Assert.Equal(result.AvailableStock, newAvailableStock);
        Assert.Equal(result.RestockThreshold, newRestockThreshold);
        Assert.Equal(result.MaxStockThreshold, newMaxStockThreshold);
        Assert.Equal(result.OnReorder, newOnRestock);
    }

    [Fact]
    public async Task UpdateItem_DoesNotExist()
    {
        var id = Guid.NewGuid().ToString();
        var newName = "RandomNewName";
        var newDescription = "RandomNewDescription";
        var newPrice = 654321;
        var newAvailableStock = 1000;
        var newRestockThreshold = 1000;
        var newMaxStockThreshold = 1000;
        var newOnRestock = true;
        var item = new CatalogItemInputDto
        {
            Name = newName,
            Description = newDescription,
            Price = newPrice,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = "Laptop",
            CatalogBrandName = "Apple",
            AvailableStock = newAvailableStock,
            RestockThreshold = newRestockThreshold,
            MaxStockThreshold = newMaxStockThreshold,
            OnReorder = newOnRestock,
        };
        var response = await _client.PutAsJsonAsync($"v1/Catalog?itemId={id}", item);

        var result = await response.Content.ReadAsStringAsync();

        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains("does not exist", result.ToLower());
    }

    [Theory]
    [InlineData("LG")]
    [InlineData("Philips")]
    public async Task UpdateItem_BrandDoesNotExist(string brand)
    {
        var id = "SpecificId1";
        var input = new CatalogItemInputDto 
        {
            Name = "RandomNewName",
            Description = "RandomNewDescription",
            Price = 654321,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = "Laptop",
            CatalogBrandName = brand,
            AvailableStock = 100,
            RestockThreshold = 150,
            MaxStockThreshold = 1000,
            OnReorder = false,
        };
        var response = await _client.PutAsJsonAsync($"v1/Catalog?itemId={id}", input);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(Regex.Match(result, @"brand.*does not exist", RegexOptions.IgnoreCase).Success);
    }
    
    [Theory]
    [InlineData("Tablet")]
    [InlineData("PC")]
    public async Task UpdateItem_TypeDoesNotExist(string type)
    {
        var id = "SpecificId1";
        var input = new CatalogItemInputDto 
        {
            Name = "RandomNewName",
            Description = "RandomNewDescription",
            Price = 654321,
            PictureFilename = "",
            PictureUri = "",
            CatalogTypeName = type,
            CatalogBrandName = "Apple",
            AvailableStock = 100,
            RestockThreshold = 150,
            MaxStockThreshold = 1000,
            OnReorder = false,
        };
        var response = await _client.PutAsJsonAsync($"v1/Catalog?itemId={id}", input);

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(Regex.Match(result, @"type.*does not exist", RegexOptions.IgnoreCase).Success);
    }

    [Fact]
    public async Task DeleteItem()
    {
        var id = "SpecificId2";
        var response = await _client.DeleteAsync($"v1/Catalog?itemId={id}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogItem>();
        
        Assert.NotNull(result);
        Assert.Equal(result.Id, id);
    }

    [Fact]
    public async Task DeleteItem_DoesNotExist()
    {
        var id = Guid.NewGuid().ToString();
        var response = await _client.DeleteAsync($"v1/Catalog?itemId={id}");

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Contains("does not exist", result);
    }

    [Theory]
    [InlineData("Dell")]
    public async Task DeleteBrand(string brandName)
    {
        var response = await _client.DeleteAsync($"v1/Catalog/brand?brandName={brandName}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogBrand>();
        
        Assert.NotNull(result);
        Assert.Equal(result.Name, brandName);
    }

    [Theory]
    [InlineData("Phone")]
    public async Task DeleteType(string typeName)
    {
        var response = await _client.DeleteAsync($"v1/Catalog/type?typeName={typeName}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CatalogType>();
        
        Assert.NotNull(result);
        Assert.Equal(result.Name, typeName);
    }
}