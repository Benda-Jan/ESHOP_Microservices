
namespace Cart.API.Payment;
public interface IPaymentClient
{
    Task<string> SendPayment(string userId);
}