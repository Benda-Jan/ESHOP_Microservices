using Cart.API.Payment;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure;
using Microsoft.AspNetCore.Mvc;


namespace Cart.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentClient _paymentClient;

    public CartController(ILogger<CartController> logger, ICartRepository cartRepository, PaymentClient paymentClient)
    {
        _logger = logger;
        _cartRepository = cartRepository;
        _paymentClient = paymentClient;
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
    public Task<IActionResult> ProcessPayment(string userId)
        => HandleAction(async () => await _paymentClient.SendPayment(userId));

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

    private async Task<IActionResult> HandleAction(Func<Task<Object?>> func)
    {
        try
        {
            var result = await func();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

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

