using System;
namespace Cart.Entities.Dtos;

public class CartItemInputDto
{
    public required string CatalogItemId { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}

