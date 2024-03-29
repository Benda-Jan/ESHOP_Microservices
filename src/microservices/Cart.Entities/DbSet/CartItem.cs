using System;

namespace Cart.Entities.DbSet;

public class CartItem
{
	public string Id { get; set; } = string.Empty;
	public required string CatalogItemId { get; set; }
	public int Quantity;
}

