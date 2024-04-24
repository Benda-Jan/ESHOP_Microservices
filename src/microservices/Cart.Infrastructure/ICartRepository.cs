using Cart.Entities.DbSet;
using Cart.Entities.Dtos;

namespace Cart.Infrastructure;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>?> GetCartItems(string userId);
    Task InsertCartItem(string userId, CartItemInputDto input);
    Task UpdateCartItem(string userId, CartItem cartItem);
    Task UpdateCatalogItem(CartItemDeserializer cartItemDeserializer);
    Task<CartItem?> DeleteCartItem(string userId, string cartItemId);
    Task DeleteCatalogItem(string cartItemId);
}

