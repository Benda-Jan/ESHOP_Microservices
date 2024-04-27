using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure;
using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.API.Test.UnitTests;

public class CartRepositoryTest
{
    [Fact]
    public async Task GetCartItems_Correct()
    {
        var context = GetContext();
        
        var userId = Guid.NewGuid().ToString();
        var cartItemId = Guid.NewGuid().ToString();
        
        var cartItem = new CartItem
        {
            Id = cartItemId,
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId,
            Name = "New Cart Item",
            Price = 123456,
            Quantity = 10
        };
        var userCart = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            CartItems = new List<CartItem>{ cartItem }
        };
        
        await context.AddAsync(userCart);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        var result = await sut.GetCartItems(userId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(cartItemId, result.ToArray()[0].Id);
    }

    [Fact]
    public async Task InsertCartItem_FirstTime_Correct()
    {
        var context = GetContext();
        var userId = Guid.NewGuid().ToString();
        var cartItemName = "New Cart Item";
        var quantity = 10;
        var cartItemDto = new CartItemInputDto
        {
	        CatalogItemId = Guid.NewGuid().ToString(),
            Name = cartItemName,
            Price = 123456,
            Quantity = quantity,
            AvailableStock = quantity + 1
        };

        var sut = new CartRepository(context);
        await sut.InsertCartItem(userId, cartItemDto);

        Assert.Single(context.CartItems);
        Assert.Equal(quantity, await context.CartItems.Where(x => x.Name == cartItemName && x.UserId == userId).Select(x => x.Quantity).SingleAsync());
    }

    [Fact]
    public async Task InsertCartItem_FirstTime_IncorrectQuantity()
    {
        var context = GetContext();
        
        var userId = Guid.NewGuid().ToString();
        var cartItemName = "New Cart Item";
        var quantity = 10;
        
        var cartItemDto = new CartItemInputDto
        {
	        CatalogItemId = Guid.NewGuid().ToString(),
            Name = cartItemName,
            Price = 123456,
            Quantity = quantity,
            AvailableStock = quantity - 1
        };

        var sut = new CartRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.InsertCartItem(userId, cartItemDto));

        Assert.Equal(0, await context.CartItems.CountAsync());
        Assert.Equal("quantity invalid", ex.Message.ToLower());
    }

    [Fact]
    public async Task InsertCartItem_SecondTime_Correct()
    {
        var context = GetContext();
       
        var userId = Guid.NewGuid().ToString();
        var cartItemName = "New Cart Item";
        var quantity = 10;
        var catalogItemId = Guid.NewGuid().ToString();
        
        var cartItemDto = new CartItemInputDto
        {
	        CatalogItemId = catalogItemId,
            Name = cartItemName,
            Price = 123456,
            Quantity = quantity,
            AvailableStock = quantity + 1
        };
        var cartItem = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = catalogItemId,
            UserId = userId,
            Name = cartItemName,
            Price = 123456,
            Quantity = quantity,
        };
        var userCart = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            CartItems = new List<CartItem>{ cartItem }
        };
        
        await context.AddAsync(userCart);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        await sut.InsertCartItem(userId, cartItemDto);

        Assert.Single(context.CartItems);
        Assert.Equal(quantity * 2, await context.CartItems.Where(x => x.Name == cartItemName && x.UserId == userId).Select(x => x.Quantity).SingleAsync());
    }

    [Fact]
    public async Task UpdateCartItem_Quantity_Correct()
    {
        var context = GetContext();
        
        var cartItemId1 = Guid.NewGuid().ToString();
        var cartItemId2 = Guid.NewGuid().ToString();
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var newQuantity = 30;
        var oldQuantity = 20;
        
        var cartItem1 = new CartItem
        {
            Id = cartItemId1,
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId1,
            Name = "cartItemName",
            Price = 123456,
            Quantity = oldQuantity,
        };
        var cartItem2 = new CartItem
        {
            Id = cartItemId2,
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId2,
            Name = "cartItemName",
            Price = 123456,
            Quantity = oldQuantity,
        };
        var userCart1 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId1,
            CartItems = new List<CartItem> { cartItem1},
        };
        var userCart2 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId2,
            CartItems = new List<CartItem> { cartItem2},
        };
        await context.AddAsync(userCart1);
        await context.AddAsync(userCart2);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        await sut.UpdateCartItem(userId1, cartItemId1, newQuantity);

        Assert.Equal(2, await context.CartItems.CountAsync());
        Assert.Equal(newQuantity, await context.CartItems.Where(x => x.Id == cartItemId1 && x.UserId == userId1).Select(x => x.Quantity).SingleAsync());
        Assert.Equal(oldQuantity, await context.CartItems.Where(x => x.Id == cartItemId2 && x.UserId == userId2).Select(x => x.Quantity).SingleAsync());
    }

    [Fact]
    public async Task UpdateCartItem_NotExists()
    {
        var context = GetContext();
        
        var cartItemId = Guid.NewGuid().ToString();
        var newQuantity = 30;
        
        var cartItem = new CartItem
        {
            Id = cartItemId,
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            Name = "cartItemName",
            Price = 123456,
            Quantity = newQuantity + 1,
        };

        var sut = new CartRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateCartItem(cartItemId, cartItem.Id, newQuantity));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task UpdateCatalogItem_Name_Correct()
    {
        var context = GetContext();
        
        var catalogItemId = Guid.NewGuid().ToString();
        var cartItemPrice = 123456;
        var cartItemId1 = Guid.NewGuid().ToString();
        var cartItemId2 = Guid.NewGuid().ToString();
        var cartItemId3 = Guid.NewGuid().ToString();
        
        var cartItem1 = new CartItem
        {
            Id = cartItemId1,
	        CatalogItemId = catalogItemId,
            UserId = Guid.NewGuid().ToString(),
            Name = "cartItemName",
            Price = cartItemPrice,
            Quantity = 10,
        };
        var cartItem2 = new CartItem
        {
            Id = cartItemId2,
	        CatalogItemId = catalogItemId,
            UserId = Guid.NewGuid().ToString(),
            Name = "cartItemName",
            Price = cartItemPrice,
            Quantity = 10,
        };
        var oldItemName = "old name";
        var cartItem3 = new CartItem
        {
            Id = cartItemId3,
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            Name = oldItemName,
            Price = cartItemPrice,
            Quantity = 10,
        };
        
        await context.AddAsync(cartItem1);
        await context.AddAsync(cartItem2);
        await context.AddAsync(cartItem3);
        await context.SaveChangesAsync();

        var newItemName = "new item name";
        var deserializedItem = new CartItemDeserializer
        {
            Id = catalogItemId,
            Name = newItemName,
            Price = cartItemPrice,
        };

        var sut = new CartRepository(context);
        await sut.UpdateCatalogItem(deserializedItem);

        Assert.Equal(newItemName, await context.CartItems.Where(x => x.Id == cartItemId1).Select(x => x.Name).SingleAsync());
        Assert.Equal(newItemName, await context.CartItems.Where(x => x.Id == cartItemId2).Select(x => x.Name).SingleAsync());
        Assert.Equal(oldItemName, await context.CartItems.Where(x => x.Id == cartItemId3).Select(x => x.Name).SingleAsync());
    }

    [Fact]
    public async Task UpdateCatalogItem_Price_Correct()
    {
        var context = GetContext();
        
        var catalogItemId = Guid.NewGuid().ToString();
        var cartItemName = "Item Name";
        var cartItemId1 = Guid.NewGuid().ToString();
        var cartItemId2 = Guid.NewGuid().ToString();
        var cartItemId3 = Guid.NewGuid().ToString();
        var newItemPrice = 654321;

        var cartItem1 = new CartItem
        {
            Id = cartItemId1,
	        CatalogItemId = catalogItemId,
            UserId = Guid.NewGuid().ToString(),
            Name = cartItemName,
            Price = newItemPrice + 1,
            Quantity = 10,
        };
        var cartItem2 = new CartItem
        {
            Id = cartItemId2,
	        CatalogItemId = catalogItemId,
            UserId = Guid.NewGuid().ToString(),
            Name = cartItemName,
            Price = newItemPrice + 1,
            Quantity = 5,
        };
        var oldItemPrice = 121212;
        var cartItem3 = new CartItem
        {
            Id = cartItemId3,
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            Name = "Some random item name",
            Price = oldItemPrice,
            Quantity = 10,
        };
        
        await context.AddAsync(cartItem1);
        await context.AddAsync(cartItem2);
        await context.AddAsync(cartItem3);
        await context.SaveChangesAsync();

        var deserializedItem = new CartItemDeserializer
        {
            Id = catalogItemId,
            Name = cartItemName,
            Price = newItemPrice,
        };

        var sut = new CartRepository(context);
        await sut.UpdateCatalogItem(deserializedItem);

        Assert.Equal(newItemPrice, await context.CartItems.Where(x => x.Id == cartItemId1).Select(x => x.Price).SingleAsync());
        Assert.Equal(newItemPrice, await context.CartItems.Where(x => x.Id == cartItemId2).Select(x => x.Price).SingleAsync());
        Assert.Equal(oldItemPrice, await context.CartItems.Where(x => x.Id == cartItemId3).Select(x => x.Price).SingleAsync());
    }

    [Fact]
    public async Task UpdateCatalogItem_NotExists()
    {
        var context = GetContext();
        
        var cartItemId = "cartItemId";
        
        var deserializedItem = new CartItemDeserializer
        {
            Id = cartItemId,
            Name = "name",
            Price = 654321,
        };

        var sut = new CartRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateCatalogItem(deserializedItem));

        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task DeleteCartItem_Correct()
    {
        var context = GetContext();
        
        var cartItemId1 = Guid.NewGuid().ToString();
        var cartItemId2 = Guid.NewGuid().ToString();
        var catalogItemId = Guid.NewGuid().ToString();
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        
        var cartItem1 = new CartItem
        {
            Id = cartItemId1,
	        CatalogItemId = catalogItemId,
            UserId = userId1,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var cartItem2 = new CartItem
        {
            Id = cartItemId2,
	        CatalogItemId = catalogItemId,
            UserId = userId2,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var userCart1 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId1,
            CartItems = new List<CartItem> { cartItem1 },
        };
        var userCart2 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId2,
            CartItems = new List<CartItem> { cartItem2 },
        };

        await context.AddAsync(userCart1);
        await context.AddAsync(userCart2);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        await sut.DeleteCartItem(userId1, cartItemId1);

        Assert.Equal(2, await context.UserCarts.CountAsync());
        Assert.NotNull(await context.CartItems.SingleOrDefaultAsync(x => x.Id == cartItemId2));
        Assert.Empty(await context.UserCarts.Where(x => x.UserId == userId1).Select(x => x.CartItems).SingleAsync());
    }

    [Fact]
    public async Task DeleteCartItem_Incorrect()
    {
        var context = GetContext();
        
        var cartItemId1 = Guid.NewGuid().ToString();
        var cartItemId2 = Guid.NewGuid().ToString();
        var catalogItemId = Guid.NewGuid().ToString();
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        
        var cartItem1 = new CartItem
        {
            Id = cartItemId1,
	        CatalogItemId = catalogItemId,
            UserId = userId1,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var userCart1 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId1,
            CartItems = new List<CartItem> { cartItem1 },
        };

        await context.AddAsync(userCart1);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.DeleteCartItem(userId2, cartItemId2));

        Assert.Null(await context.CartItems.SingleOrDefaultAsync(x => x.Id == cartItemId2));
        Assert.Single(await context.UserCarts.Where(x => x.UserId == userId1).Select(x => x.CartItems).SingleAsync());
        Assert.Null(await context.UserCarts.SingleOrDefaultAsync(x => x.UserId == userId2));
        Assert.Contains("does not exist", ex.Message.ToLower());
    }

    [Fact]
    public async Task DeleteCatalogItem_Correct()
    {
        var context = GetContext();
        
        var catalogItemId = Guid.NewGuid().ToString();
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        
        var cartItem1 = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = catalogItemId,
            UserId = userId1,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var cartItem2 = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId1,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var cartItem3 = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = catalogItemId,
            UserId = userId2,
            Name = "name2",
            Price = 654321,
            Quantity = 10,
        };
        var userCart1 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId1,
            CartItems = new List<CartItem> { cartItem1, cartItem2 },
        };
        var userCart2 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId2,
            CartItems = new List<CartItem> { cartItem3 },
        };

        await context.AddAsync(userCart1);
        await context.AddAsync(userCart2);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        await sut.DeleteCatalogItem(catalogItemId);

        Assert.Single(await context.UserCarts.Where(x => x.UserId == userId1).Select(x => x.CartItems).SingleAsync());
        Assert.Empty(await context.UserCarts.Where(x => x.UserId == userId2).Select(x => x.CartItems).SingleAsync());
        Assert.Single(context.CartItems);
    }

    [Fact]
    public async Task DeleteCatalogItem_Correct_NotExists()
    {
        var context = GetContext();
        
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();

        var cartItem1 = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId1,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var cartItem2 = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId1,
            Name = "name",
            Price = 123456,
            Quantity = 10,
        };
        var cartItem3 = new CartItem
        {
            Id = Guid.NewGuid().ToString(),
	        CatalogItemId = Guid.NewGuid().ToString(),
            UserId = userId2,
            Name = "name2",
            Price = 654321,
            Quantity = 10,
        };
        var userCart1 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId1,
            CartItems = new List<CartItem> { cartItem1, cartItem2 },
        };
        var userCart2 = new UserCart
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId2,
            CartItems = new List<CartItem> { cartItem3 },
        };

        await context.AddAsync(userCart1);
        await context.AddAsync(userCart2);
        await context.SaveChangesAsync();

        var sut = new CartRepository(context);
        await sut.DeleteCatalogItem(Guid.NewGuid().ToString());

        Assert.Equal(2, await context.UserCarts.Where(x => x.UserId == userId1).SelectMany(x => x.CartItems).CountAsync());
        Assert.Single(await context.UserCarts.Where(x => x.UserId == userId2).Select(x => x.CartItems).SingleAsync());
        Assert.Equal(3, await context.CartItems.CountAsync());
    }

    private CartContext GetContext()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<CartContext>();
        builder.UseInMemoryDatabase("InMemoryForTesting").UseInternalServiceProvider(serviceProvider);

        var context = new CartContext(builder.Options);
        return context;
    }
}