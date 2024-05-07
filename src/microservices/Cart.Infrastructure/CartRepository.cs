using System.Text.Json;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure;

public class CartRepository(CartContext cartContext) : ICartRepository
{
    private readonly CartContext _cartContext = cartContext;
    
    public async Task<IEnumerable<CartItem>?> GetCartItems(string userId)
        => await _cartContext.UserCarts
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.CartItems)
            .SingleOrDefaultAsync();

    public async Task<UserCart?> InsertCartItem(string userId, CartItemInputDto input)
    {
        if (input.Quantity >= input.AvailableStock)
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
                UserId = userId,
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

        return userCart;
    }

    public async Task<CartItem?> UpdateCartItem(string userId, string cartItemId, int quantity)
    {
        var cartItem = await _cartContext.UserCarts
            .Where(x => x.UserId == userId)
            .SelectMany(x => x.CartItems)
            .SingleOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId)
            ?? throw new Exception("Cart item does not exist in user's cart");

        cartItem.Quantity = quantity;

        _cartContext.CartItems.Update(cartItem);
        await _cartContext.SaveChangesAsync();

        return cartItem;
    }

    public async Task UpdateCatalogItem(CartItemDeserializer cartItemDeserializer)
    {
        var cartItemsQuery = _cartContext.CartItems.Where(x => x.CatalogItemId == cartItemDeserializer.Id);

        if (await cartItemsQuery.CountAsync() == 0)
            throw new Exception("Cart item does not exist");

        foreach (var cartItem in cartItemsQuery)
        {
            cartItem.Name = cartItemDeserializer.Name;
            cartItem.Price = cartItemDeserializer.Price;

            _cartContext.CartItems.Update(cartItem);
            await _cartContext.SaveChangesAsync();
        }
    }

    public async Task<CartItem?> DeleteCartItem(string userId, string cartItemId)
    {
        var userCart = await _cartContext.UserCarts.Where(x => x.UserId == userId).Include(x => x.CartItems).SingleOrDefaultAsync()
            ?? throw new Exception("User Cart does not exist");

        var cartItem = userCart.CartItems.SingleOrDefault(x => x.Id == cartItemId);
        if (cartItem != null)
        {
            userCart.CartItems?.Remove(cartItem);
            _cartContext.UserCarts.Update(userCart);
            await _cartContext.SaveChangesAsync();

            cartItem.Quantity = -cartItem.Quantity;
        } 
        else
        {
            throw new Exception("Cart item does not exist in user's cart");
        }
        return cartItem;
    }

    public async Task DeleteCatalogItem(string catalogItemId)
    {
        var cartItemsQuery = _cartContext.CartItems.Where(x => x.CatalogItemId == catalogItemId);
        
        foreach (var cartItem in cartItemsQuery)
        {
            _cartContext.CartItems.Remove(cartItem);
            await _cartContext.SaveChangesAsync();
        }
    }
}

