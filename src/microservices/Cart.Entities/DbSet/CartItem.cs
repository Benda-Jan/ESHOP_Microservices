using System;

namespace Cart.Entities.DbSet;

public class CartItem
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public required string CatalogItemId { get; set; }
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }
}

public class CartItemDeserializer{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
}

public class CartItemSerializer{
    public required string CatalogItemId { get; set; }
    public required int Quantity { get; set; }
}

