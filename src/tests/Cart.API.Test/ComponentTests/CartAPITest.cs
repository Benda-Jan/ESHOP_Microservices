
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace Cart.API.Test.ComponentTests;

public class CartAPITest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CartAPITest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");
    }

    [Fact]
    public async Task GetCartItems()
    {
        var userId = "User1";
        var response = await _client.GetAsync($"v1/Cart/user/{userId}/items");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CartItem[]>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains("SpecificId1", result.Select(x => x.Id));
        Assert.Contains("SpecificId2", result.Select(x => x.Id));
    }

    [Fact]
    public async Task CreateCartItem()
    {
        var userId = Guid.NewGuid().ToString();
        var itemName = "NewRandomName";
        var cartInputDto = new CartItemInputDto
        {
            CatalogItemId = Guid.NewGuid().ToString(),
            Name = itemName,
            Price = 3243424,
            Quantity = 1,
            AvailableStock = 100,
        };
        var response = await _client.PostAsJsonAsync($"v1/Cart/user/{userId}/item", cartInputDto);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UserCart>();
        Assert.NotNull(result);
        Assert.Equal(result.UserId, userId);
        Assert.Contains(itemName, result.CartItems.Select(x => x.Name));
    }

    [Fact]
    public async Task CreateCartItem_IncorrectQuantity()
    {
        var userId = Guid.NewGuid().ToString();
        var itemName = "NewRandomName";
        var Quantity = 10;
        var cartInputDto = new CartItemInputDto
        {
            CatalogItemId = Guid.NewGuid().ToString(),
            Name = itemName,
            Price = 3243424,
            Quantity = Quantity,
            AvailableStock = Quantity - 1,
        };
        var response = await _client.PostAsJsonAsync($"v1/Cart/user/{userId}/item", cartInputDto);

        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
        Assert.Contains("quantity invalid", result.ToLower());
    }

    [Fact]
    public async Task UpdateCartItem()
    {
        var userId = "User1";
        var cartItemId = "SpecificId1";
        int newQuantity = 20;
        var response = await _client.PutAsJsonAsync($"v1/Cart/user/{userId}/item?cartItemId={cartItemId}", newQuantity);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CartItem>();
        Assert.NotNull(result);
        Assert.Equal(result.UserId, userId);
        Assert.Equal(result.Id, cartItemId);
        Assert.Equal(result.Quantity, newQuantity);
    }

    [Fact]
    public async Task UpdateCartItem_CartItem_DoesNotExist()
    {
        var userId = "User1";
        var cartItemId = Guid.NewGuid().ToString();
        int newQuantity = 20;
        var response = await _client.PutAsJsonAsync($"v1/Cart/user/{userId}/item?cartItemId={cartItemId}", newQuantity);

        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
        Assert.Contains("does not exist", result);
    }

    [Fact]
    public async Task UpdateCartItem_UserCart_DoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();
        var cartItemId = "SpecificId1";
        int newQuantity = 20;
        var response = await _client.PutAsJsonAsync($"v1/Cart/user/{userId}/item?cartItemId={cartItemId}", newQuantity);

        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
        Assert.Contains("does not exist", result);
    }

    [Fact]
    public async Task DeleteItem()
    {
        var userId = "User2";
        var cartItemId = "SpecificId4";

        var response = await _client.DeleteAsync($"v1/Cart/user/{userId}/item/{cartItemId}");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CartItem>();
        
        Assert.NotNull(result);
        Assert.Equal(cartItemId, result.Id);
    }

    [Fact]
    public async Task DeleteItem_CartItem_DoesNotExist()
    {
        var userId = "User2";
        var cartItemId = Guid.NewGuid().ToString();

        var response = await _client.DeleteAsync($"v1/Cart/user/{userId}/item/{cartItemId}");

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.NotNull(result);
        Assert.Contains("does not exist", result);
    }

    [Fact]
    public async Task DeleteItem_UserCart_DoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();
        var cartItemId = "SpecificId4";

        var response = await _client.DeleteAsync($"v1/Cart/user/{userId}/item/{cartItemId}");

        var result = await response.Content.ReadAsStringAsync();
        
        Assert.NotNull(result);
        Assert.Contains("does not exist", result);
    }
}