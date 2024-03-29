﻿using System;

namespace Cart.Entities.DbSet;

public class UserCart
{
	public string Id { get; set; } = string.Empty;
	public required string UserId { get; set; }
	public List<CartItem>? CartItems { get; set; }
}
