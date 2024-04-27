using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Identity.Entities.DbSet;

namespace Identity.API.Test.UnitTests;

public class UserRepositoryTest
{
    [Fact]
    public async Task GetUser_Correct()
    {
        var context = GetContext();

        var username = "username";
        var email = "email@email.com";
        var password = "password";

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context);
        var result = await sut.GetUser(email);

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetUser_NotExists()
    {
        var context = GetContext();

        var email = "email@email.com";

        var sut = new UserRepository(context);
        var result = await sut.GetUser(email);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertUser_First_Correct()
    {
        var context = GetContext();

        var username = "username";
        var email = "email@email.com";
        var password = "password";

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };

        var sut = new UserRepository(context);
        await sut.InsertUser(user);

        Assert.NotNull(context.Users);
        Assert.Equal(username, await context.Users.Where(x => x.Email == email).Select(x => x.Username).SingleAsync());
    }

    [Fact]
    public async Task InsertUser_Second_AlreadyExists()
    {
        var context = GetContext();

        var username = "username1";
        var email = "email1@email.com";
        var password = "password";

        var user1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };
        var user2 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            Password = password,
            Salt = "kjdsbfjndsf9ew743y5dsbjfn",
            IsAdmin = false,
        };
        await context.Users.AddAsync(user1);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.InsertUser(user2));

        Assert.Single(context.Users);
        Assert.Equal(username, await context.Users.Where(x => x.Email == email).Select(x => x.Username).SingleAsync());
        Assert.Contains("already exists", ex.Message.ToLower());
    }

    [Fact]
    public async Task InsertUser_Second_Correct()
    {
        var context = GetContext();

        var username1 = "username1";
        var email1 = "email1@email.com";
        var username2 = "username2";
        var email2 = "email2@email.com";
        var password = "password";

        var user1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username1,
            Email = email1,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };
        var user2 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username2,
            Email = email2,
            Password = password,
            Salt = "kjdsbfjndsf9ew743y5dsbjfn",
            IsAdmin = false,
        };
        await context.Users.AddAsync(user1);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context);
        await sut.InsertUser(user2);

        Assert.Equal(2, await context.Users.CountAsync());
        Assert.Equal(username1, await context.Users.Where(x => x.Email == email1).Select(x => x.Username).SingleAsync());
        Assert.Equal(username2, await context.Users.Where(x => x.Email == email2).Select(x => x.Username).SingleAsync());
    }

    private UserContext GetContext()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<UserContext>();
        builder.UseInMemoryDatabase("InMemoryForTesting").UseInternalServiceProvider(serviceProvider);

        var context = new UserContext(builder.Options);
        return context;
    }
}