using Identity.Entities;
using Identity.Entities.DbSet;
using Identity.Entities.Dtos;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using JwtLibrary;

namespace Identity.Infrastructure;

public class UserRepository(UserContext userContext, IEncryptor encryptor, IJwtBuilder jwtBuilder) : IUserRepository
{
    private readonly UserContext _userContext = userContext;
    private readonly IEncryptor _encryptor = encryptor;
    private readonly IJwtBuilder _jwtBuilder = jwtBuilder;

    public Task<User?> FindByEmail(string email)
        => _userContext.Users.SingleOrDefaultAsync(x => x.Email == email);

    public Task<User?> FindByUsername(string username)
        => _userContext.Users.SingleOrDefaultAsync(x => x.Username == username);

    public async Task<TokenOutputDto> ValidateUser(LoginInputDto inputDto)
    {
        var user = await FindByEmail(inputDto.Email);

        if (user == null)
            throw new Exception($"User with email: {inputDto.Email} does not exist");

        if (!user.ValidatePassword(inputDto.Password, _encryptor))
            throw new Exception("Incorrect password");

        var tokenDto = new TokenOutputDto
        {
            Token = _jwtBuilder.GetToken(user.Id!),
            RefreshToken = "",
        };

        return tokenDto;
    }

    public async Task<TokenOutputDto> InsertUser(RegisterInputDto inputDto)
    {
        if (await FindByEmail(inputDto.Email) != null)
            throw new Exception($"User with email: {inputDto.Email} already exists");
        
        if (await FindByUsername(inputDto.Username) != null)
            throw new Exception($"User with username: {inputDto.Username} already exists");

        if (inputDto.Password != inputDto.PasswordConfirmation)
            throw new Exception("Passwords are different");

        var newUser = new User(inputDto.Username, inputDto.Email, inputDto.Password, _encryptor);

        await _userContext.Users.AddAsync(newUser);
        await _userContext.SaveChangesAsync();

        var tokenDto = new TokenOutputDto
        {
            Token = _jwtBuilder.GetToken(newUser.Id!),
            RefreshToken = "",
        };

        return tokenDto;
    }
}

