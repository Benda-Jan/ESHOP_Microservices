using Cart.Infrastructure;
using Grpc.Net.Client;
using Microsoft.IdentityModel.Tokens;
using Payment.API;
using static Payment.API.PaymentManager;

namespace Cart.API.Payment;

public class PaymentClient(string connection, ICartRepository cartRepository) : IPaymentClient
{   
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly PaymentManagerClient _paymentManagerClient = new PaymentManagerClient(GrpcChannel.ForAddress(connection));

    public async Task<string> SendPayment(string userId)
    {
        var cartItems = await _cartRepository.GetCartItems(userId);

        if (cartItems.IsNullOrEmpty())
            throw new BadHttpRequestException("User Cart is empty");

        var amount = (long)cartItems!.Select(x => x.Price * x.Quantity).Sum();

        var reply =  await _paymentManagerClient.SendPaymentAsync(new PaymentRequest { Amount = amount });

        return reply.Message;        
    }
}