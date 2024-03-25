using Identity.Entities.DbSet;
using Identity.Entities.Dtos;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly UserContext _userContext;

    public UserRepository(UserContext userContext)
    {
        _userContext = userContext;
    }

    public Task<User?> GetUser(string email)
        => _userContext.Users.SingleOrDefaultAsync(x => x.Email == email);

    public async Task InsertUser(User user)
    {
        var result = await _userContext.Users.SingleOrDefaultAsync(x => x.Email == user.Email);

        if (result is not null)
            throw new Exception($"User with email: {user.Email} already exists");

        await _userContext.Users.AddAsync(user);
        await _userContext.SaveChangesAsync();
    }
}

