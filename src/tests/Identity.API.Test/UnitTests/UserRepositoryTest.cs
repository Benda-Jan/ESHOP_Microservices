using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Identity.Entities.DbSet;
using Identity.Entities.Dtos;
using Identity.Entities;
using JwtLibrary;
using Microsoft.VisualBasic;

namespace Identity.API.Test.UnitTests;

public class UserRepositoryTest
{
    [Theory]
    [InlineData("User1@email.com")]
    [InlineData("User2@email.com")]
    public async Task FindByEmail_Correct(string email)
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username = "user";
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
        await context.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var result = await sut.FindByEmail(email);

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task FindByEmail_UserNotExists()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var result =  await sut.FindByEmail("RandomNotExistingEmail");

        Assert.Null(result);
    }

        [Theory]
    [InlineData("User1")]
    [InlineData("User2")]
    public async Task FindByUsername_Correct(string username)
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var email = "user@email.com";
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
        await context.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var result = await sut.FindByUsername(username);

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task FindByUsername_UserNotExists()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var result =  await sut.FindByUsername("RandomNotExistingUsername");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("User1@email.com")]
    [InlineData("User2@email.com")]
    public async Task ValidateUser_Correct(string email)
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var password = "SecretPassword123@";
        var username = "User";
        var user = new User (username, email, password, encryptor);
        var userDto = new LoginInputDto
        {
            Email = email,
            Password = password,
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var result = await sut.ValidateUser(userDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ValidateUser_NotExists()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var password = "SecretPassword123@";
        var email = "RandomNotExistingEmail@email.com";
        var userinput = new LoginInputDto
        {
            Email = email,
            Password = password,
        };

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.ValidateUser(userinput));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Theory]
    [InlineData("User1@email.com")]
    [InlineData("User2@email.com")]
    public async Task ValidateUser_IncorrectPassword(string email)
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username = "User";
        var passwordCorrect = "Password123@";
        var passwordIncorrect = "RandomIncorrectPassword";
        var user = new User (username, email, passwordCorrect, encryptor);
        var userinput = new LoginInputDto
        {
            Email = email,
            Password = passwordIncorrect,
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.ValidateUser(userinput));

        Assert.Contains("incorrect password", ex.Message.ToLower());
    }

    [Fact]
    public async Task InsertUser_First_Correct()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username = "username";
        var email = "email@email.com";
        var password = "password";

        var user = new RegisterInputDto
        {
            Username = username,
            Email = email,
            Password = password,
            PasswordConfirmation = password,
        };

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        await sut.InsertUser(user);

        Assert.NotNull(context.Users);
        Assert.Equal(username, await context.Users.Where(x => x.Email == email).Select(x => x.Username).SingleAsync());
    }

    [Fact]
    public async Task InsertUser_Second_EmailAlreadyExists()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username1 = "username1";
        var username2 = "username2";
        var email = "email@email.com";
        var password = "password";

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username1,
            Email = email,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };
        var userDto = new RegisterInputDto
        {
            Username = username2,
            Email = email,
            Password = password,
            PasswordConfirmation = password,
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.InsertUser(userDto));

        Assert.Equal(username1, await context.Users.Where(x => x.Email == email).Select(x => x.Username).SingleAsync());
        Assert.Contains("already exists", ex.Message.ToLower());
    }

    [Fact]
    public async Task InsertUser_Second_UsernameAlreadyExists()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username = "username";
        var email1 = "email1@email.com";
        var email2 = "email2@email.com";
        var password = "password";

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email1,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };
        var userDto = new RegisterInputDto
        {
            Username = username,
            Email = email2,
            Password = password,
            PasswordConfirmation = password,
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.InsertUser(userDto));

        Assert.Equal(username, await context.Users.Where(x => x.Email == email1).Select(x => x.Username).SingleAsync());
        Assert.Contains("already exists", ex.Message.ToLower());
    }

    [Fact]
    public async Task InsertUser_Second_Correct()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username1 = "username1";
        var email1 = "email1@email.com";
        var username2 = "username2";
        var email2 = "email2@email.com";
        var password = "password";

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username1,
            Email = email1,
            Password = password,
            Salt = "kjdsbfjndsf9743y5dsbjfn",
            IsAdmin = false,
        };
        var userDto = new RegisterInputDto
        {
            Username = username2,
            Email = email2,
            Password = password,
            PasswordConfirmation = password,
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        await sut.InsertUser(userDto);

        Assert.Equal(2, await context.Users.CountAsync());
        Assert.Equal(username1, await context.Users.Where(x => x.Email == email1).Select(x => x.Username).SingleAsync());
        Assert.Equal(username2, await context.Users.Where(x => x.Email == email2).Select(x => x.Username).SingleAsync());
    }

    [Fact]
    public async Task InsertUser_DifferentPasswords()
    {
        var context = GetContext();
        var encryptor = GetEncryptor();
        var jwtBuilder = GetJwtBuilder();

        var username = "username";
        var email = "email@email.com";
        var password = "password";
        var passwordConfirmation = "passwordConfirmation";

        var userDto = new RegisterInputDto
        {
            Username = username,
            Email = email,
            Password = password,
            PasswordConfirmation = passwordConfirmation,
        };

        var sut = new UserRepository(context, encryptor, jwtBuilder);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.InsertUser(userDto));

        Assert.Contains("different", ex.Message.ToLower());
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

    private IEncryptor GetEncryptor()
        => new Encryptor();

    private IJwtBuilder GetJwtBuilder()
    {
        var options = new JwtOptions
        {
            ExpiryMinutes = 10,
            SecretKey = "sadsdas98987as897as8das'.\'/3423234'/234'/23",
        };
        IOptions<JwtOptions> jwtOptions = Options.Create(options);
        var jwtBuilder = new JwtBuilder(jwtOptions);

        return jwtBuilder;
    }
}