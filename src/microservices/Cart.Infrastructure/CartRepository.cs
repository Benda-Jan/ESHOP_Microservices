using System.Text.Json;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure;

public class CartRepository : ICartRepository
{
    private readonly CartContext _cartContext;

	public CartRepository(CartContext cartContext)
	{
        _cartContext = cartContext;
	}

    public async Task<CartItem?> DeleteCartItem(string userId, string cartItemId)
    {
        var userCart = await _cartContext.UserCarts.Where(x => x.UserId == userId).Include(x => x.CartItems).FirstOrDefaultAsync()
            ?? throw new Exception("User Cart does not exist");

        var cartItem = userCart.CartItems?.SingleOrDefault(x => x.Id == cartItemId);
        if (cartItem != null)
        {
            userCart.CartItems?.Remove(cartItem);
            _cartContext.UserCarts.Update(userCart);
            await _cartContext.SaveChangesAsync();

            cartItem.Quantity = -cartItem.Quantity;
        }
        return cartItem;
    }

    public async Task DeleteCatalogItem(string cartItemId)
    {
        var cartItem = await _cartContext.CartItems.SingleOrDefaultAsync(x => x.CatalogItemId == cartItemId)
            ?? throw new Exception("Cart item does not exist");

            _cartContext.CartItems.Remove(cartItem);
            await _cartContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<CartItem>?> GetCartItems(string userId)
    {
        var userCart = await _cartContext.UserCarts.AsNoTracking().Where(x => x.UserId == userId).Include(x => x.CartItems).SingleOrDefaultAsync();

        return userCart?.CartItems;
    }

    public async Task InsertCartItem(string userId, CartItemInputDto input)
    {
        // check quantity
        if (input.Quantity > input.AvailableStock)
            throw new Exception("Quantity invalid");

        var userCart = await _cartContext.UserCarts.Where(x => x.UserId == userId).Include(x => x.CartItems).SingleOrDefaultAsync();
        bool userCartExists = userCart != null;
        if (userCart is null)
        {
            userCart = new UserCart()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                CartItems = new List<CartItem>()
            };
        }

        if (userCart.CartItems is null)
            userCart.CartItems = new List<CartItem>();

        var cartItem = userCart.CartItems.SingleOrDefault(x => x.CatalogItemId == input.CatalogItemId);
        if (cartItem is not null)
        {
            cartItem.Quantity += input.Quantity;
        }
        else
        {
            cartItem = new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                CatalogItemId = input.CatalogItemId,
                Name = input.Name,
                Price = input.Price,
                Quantity = input.Quantity
            };
            userCart.CartItems.Add(cartItem);
        }

        if (userCartExists)
            _cartContext.UserCarts.Update(userCart);
        else
            await _cartContext.UserCarts.AddAsync(userCart);

        await _cartContext.SaveChangesAsync();
    }

    public async Task UpdateCartItem(string userId, CartItem cartItem)
    {
        _cartContext.CartItems.Update(cartItem);
        await _cartContext.SaveChangesAsync();
    }

    public async Task UpdateCatalogItem(CartItemDeserializer cartItemDeserializer)
    {
        var cartItem = _cartContext.CartItems.FirstOrDefault(x => x.CatalogItemId == cartItemDeserializer.Id);
        if (cartItem == null)
            return;

        cartItem.Name = cartItemDeserializer.Name;
        cartItem.Price = cartItemDeserializer.Price;

        _cartContext.CartItems.Update(cartItem);
        await _cartContext.SaveChangesAsync();
    }
}

