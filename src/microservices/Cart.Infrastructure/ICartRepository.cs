using Cart.Entities.DbSet;
using Cart.Entities.Dtos;

namespace Cart.Infrastructure;

public interface ICartRepository
{
    Task<IList<CartItem>> GetCartItems(string userId);
    Task InsertCartItem(string userId, CartItemInputDto input);
    Task UpdateCartItem(string userId, CartItem cartItem);
    Task DeleteCartItem(string userId, string cartItemId);
    //void UpdateCatalogItem(string catalogItemId, string name, decimal price);
    //void DeleteCatalogItem(string catalogItemId);
}

