using Identity.Entities.DbSet;
using Identity.Entities.Dtos;

namespace Identity.Infrastructure;

public interface IUserRepository
{
    Task<User?> FindByEmail(string email);
    Task<User?> FindByUsername(string username);
    Task<TokenOutputDto> ValidateUser(LoginInputDto inputDto);
    Task<TokenOutputDto> InsertUser(RegisterInputDto inputDto);
}

