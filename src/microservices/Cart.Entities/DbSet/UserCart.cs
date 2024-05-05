using System;

namespace Cart.Entities.DbSet;

public class UserCart
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required string UserId { get; set; }
	public IList<CartItem> CartItems { get; set; } = new List<CartItem>();
}

