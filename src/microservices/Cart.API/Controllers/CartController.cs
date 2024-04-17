using System.Net;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure;
using Microsoft.AspNetCore.Mvc;

using Grpc.Net.Client;
using Microsoft.IdentityModel.Tokens;

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
    //[Authorize]
    [Route("user/{userId}/items")]
    public Task<IActionResult> GetItems(string userId)
        => HandleAction(async () => await _cartRepository.GetCartItems(userId));

    [HttpPost]
    //[Authorize]
    [Route("user/{userId}/item")]
    public Task<IActionResult> CreateItem(string userId, CartItemInputDto cartItem)
        => HandleAction(async () => await _cartRepository.InsertCartItem(userId, cartItem));

    [HttpPost]
    //[Authorize]
    [Route("user/{userId}/ProcessPayment")]
    public async Task<IActionResult> ProcessPayment(string userId)
    {
        var amountQuery = await _cartRepository.GetCartItems(userId);

        if (amountQuery.IsNullOrEmpty())
            return BadRequest("User Cart is empty");

        var amount = (long)amountQuery!.Select(x => x.Price * x.Quantity).Sum();

        using var channel = GrpcChannel.ForAddress("http://localhost:5059");
        var client = new PaymentManager.PaymentManagerClient(channel);

        var reply =  await client.SendPaymentAsync(new PaymentRequest { Amount = amount });

        return Ok(reply.Message);
    }

    [HttpPut]
    //[Authorize]
    [Route("user/{userId}/item")]
    public Task<IActionResult> UpdateItem(string userId, CartItem cartItem)
        => HandleAction(async () => await _cartRepository.UpdateCartItem(userId, cartItem));

    [HttpDelete]
    //[Authorize]
    [Route("user/{userId}/item/{cartItemId}")]
    public Task<IActionResult> DeleteItem(string userId, string cartItemId)
        => HandleAction(async () => await _cartRepository.DeleteCartItem(userId, cartItemId));

    private async Task<IActionResult> HandleAction(Func<Task> func)
    {
        try
        {
            await func();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

