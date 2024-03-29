using System.Net;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly ICartRepository _cartRepository;

    public CartController(ILogger<CartController> logger, ICartRepository cartRepository)
    {
        _logger = logger;
        _cartRepository = cartRepository;
    }

    [HttpGet]
    [Authorize]
    [Route("user/{userId}/items")]
    [ProducesResponseType(typeof(IEnumerable<CartItem>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<CartItem>>> GetItems(string userId)
    {
        var items = await _cartRepository.GetCartItems(userId);
        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    [Route("user/{userId}/item")]
    public async Task<IActionResult> CreateItem(string userId, CartItemInputDto cartItem)
    {
        await _cartRepository.InsertCartItem(userId, cartItem);
        return Ok();
    }

    [HttpPut]
    [Authorize]
    [Route("user/{userId}/item")]
    public async Task<IActionResult> UpdateItem(string userId, CartItem cartItem)
    {
        await _cartRepository.UpdateCartItem(userId, cartItem);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    [Route("user/{userId}/item/{cartId}")]
    public async Task<IActionResult> DeleteItem(string userId, string cartId)
    {
        await _cartRepository.DeleteCartItem(userId, cartId);
        return Ok();
    }
}

