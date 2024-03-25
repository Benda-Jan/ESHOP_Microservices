using System;
using Identity.Entities.DbSet;

namespace Identity.Infrastructure;

public interface IUserRepository
{
    Task<User?> GetUser(string email);
    Task InsertUser(User user);
}

