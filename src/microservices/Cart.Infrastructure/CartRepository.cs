using System;
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

    public async Task DeleteCartItem(string userId, string cartItemId)
    {
        var cartItem = await _cartContext.CartItems.SingleOrDefaultAsync(x => x.Id == cartItemId)
            ?? throw new Exception("Cart item does not exist");

        _cartContext.CartItems.Remove(cartItem);
        await _cartContext.SaveChangesAsync();
    }

    public async Task<IList<CartItem>> GetCartItems(string userId)
    {
        var userCart = await _cartContext.UserCarts.AsNoTracking().Where(x => x.UserId == userId).Include(x => x.CartItems).SingleOrDefaultAsync();

        return userCart?.CartItems ?? new List<CartItem>();
    }

    public async Task InsertCartItem(string userId, CartItemInputDto input)
    {
        var userCart = await _cartContext.UserCarts.SingleOrDefaultAsync(x => x.UserId == userId);
        bool exists = userCart != null;
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

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
            CatalogItemId = input.CatalogItemId,
            Quantity = 1
        };

        userCart.CartItems.Add(cartItem);

        if (exists)
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

}

