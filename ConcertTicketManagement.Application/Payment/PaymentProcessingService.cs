
using ConcertTicketManagement.Contracts.Payments;

namespace ConcertTicketManagement.Application.Payment
{
    public sealed class PaymentProcessingService : IPaymentProcessingService
    {
        public Task<string> ProcessPayment(decimal paymentAmount, PaymentMethodInformation paymentMethod)
        {
            return Task.Run(() =>
            {
                // Simulate payment processing logic
                if (paymentAmount <= 0)
                {
                    throw new InvalidOperationException("Payment amount must be greater than zero.");
                }

                // For this example, we will assume the payment is always successful and return transactionID
                return "12345";
            });
        }

        public Task<bool> RevertPayment(string paymentId)
        {
            return Task.FromResult(true); // Simulate successful payment reversal
        }
    }
}
