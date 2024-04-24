using System.Text.Json;
using Cart.API.EventsHandling;
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
    private readonly EventBusCartItemUpdated _eventBusPublisher;

    public CartController(ICartRepository cartRepository, IPaymentClient paymentClient, EventBusCartItemUpdated eventBusPublisher)
    {
        _cartRepository = cartRepository;
        _paymentClient = paymentClient;
        _eventBusPublisher = eventBusPublisher;
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
        => HandleAction(async () => 
        {
            await _cartRepository.InsertCartItem(userId, cartItem);
            _eventBusPublisher.Publish(JsonSerializer.Serialize(new CartItemSerializer {CatalogItemId = cartItem.CatalogItemId, Quantity = cartItem.Quantity}));
            return Task.CompletedTask;
        });

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
        => HandleAction(async () => 
        {
            var result = await _cartRepository.DeleteCartItem(userId, cartItemId);
            if (result != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(result));
            return Ok(result);
        });

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

