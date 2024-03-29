using System;
using Cart.Entities.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Data;

public class CartContext : DbContext
{
	public DbSet<CartItem> CartItems { get; set; } = null!;
	public DbSet<UserCart> UserCarts { get; set; } = null!;

	public CartContext(DbContextOptions<CartContext> options) : base(options)
	{
	}
}

