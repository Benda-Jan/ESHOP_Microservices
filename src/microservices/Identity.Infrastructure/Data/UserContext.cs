using System;
using Identity.Entities.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }
}

