
using System.Net.Http.Json;
using System.Reflection;
using Identity.API;
using Identity.Entities.DbSet;
using Identity.Entities.Dtos;

namespace Identity.API.Test.ComponentTests;

public class IdentityAPITest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IdentityAPITest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("User1@email.com", "SecretPassword123@")]
    [InlineData("User2@email.com", "SecretPassword123@")]
    public async Task Login(string email, string password)
    {
        var input = new LoginInputDto
        {
            Email = email,
            Password = password,
        };
        var response = await _client.PostAsJsonAsync($"v1/Identity/login", input);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TokenOutputDto>();
        
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);
    }
}