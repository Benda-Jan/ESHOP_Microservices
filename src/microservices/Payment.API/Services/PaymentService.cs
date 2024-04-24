using Grpc.Core;
using Stripe;

namespace Payment.API.Services;

public class PaymentService : PaymentManager.PaymentManagerBase
{
    public PaymentService()
    {
    }

    public override Task<PaymentReply> SendPayment(PaymentRequest request, ServerCallContext context)
    {
        
        // Nastavení API klíče Stripe
        StripeConfiguration.ApiKey = "sk_test_51OYtg8JpDeAr1xRHAEMzqAQT5rS8wt57T7MgblKOW8QjIDvqtPjz7E9G9lxb1efE9XiyfdtZhr4nGDHI9TolMuyJ00DeB1UxBz";

        var method = new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions
            {
                Number = "4242 4242 4242 4242",
                ExpMonth = 12,
                ExpYear = 24,
                Cvc = "567",
            },
        };
        var methodService = new PaymentMethodService();
        //var paymentMethod = methodService.Create(method);

        // Volání metody Stripe API pro zpracování platby
        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount,
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "card" },
            PaymentMethod = "pm_card_visa",
        };

        var service = new PaymentIntentService();
        var paymentIntent = service.Create(options);


        var confirmOptions = new PaymentIntentConfirmOptions
        {
            PaymentMethod = "pm_card_visa",
        };
        var paymentIntentConfirm = service.Confirm(paymentIntent.Id, confirmOptions);

        return Task.FromResult(new PaymentReply
        {
            Message = "PaymentManager repsonse: " + paymentIntentConfirm.Status + " " + request.Amount + " paid"
        });
    }
}

