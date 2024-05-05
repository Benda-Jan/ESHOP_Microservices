using Cart.Entities.DbSet;
using Cart.Entities.Dtos;

namespace Cart.Infrastructure;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>?> GetCartItems(string userId);
    Task<UserCart?> InsertCartItem(string userId, CartItemInputDto input);
    Task<CartItem?> UpdateCartItem(string userId, string cartItemId, int quantity);
    Task UpdateCatalogItem(CartItemDeserializer cartItemDeserializer);
    Task<CartItem?> DeleteCartItem(string userId, string cartItemId);
    Task DeleteCatalogItem(string catalogItemId);
}

