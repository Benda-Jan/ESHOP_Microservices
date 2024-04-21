using Cart.Infrastructure;
using Grpc.Net.Client;
using static Cart.API.PaymentManager;
using Microsoft.IdentityModel.Tokens;

namespace Cart.API.Payment;

public class PaymentClient : IPaymentClient
{   
    private readonly ICartRepository _cartRepository;
    private readonly PaymentManagerClient _paymentManagerClient;

    public PaymentClient(string connection, ICartRepository cartRepository)
    {   
        _cartRepository = cartRepository;
        var channel = GrpcChannel.ForAddress(connection);
        _paymentManagerClient = new PaymentManagerClient(channel);
    }

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