using ConcertTicketManagement.Contracts.Payments;

namespace ConcertTicketManagement.Application.Payment
{
    public interface IPaymentProcessingService
    {
        /// <summary>
        /// Processes a payment for the specified amount using the provided payment method.
        /// </summary>
        /// <param name="paymentAmount">Payment amount</param>
        /// <param name="paymentMethod">Payment Method</param>
        /// <returns></returns>
        Task<bool> ProcessPayment(decimal paymentAmount, PaymentMethod paymentMethod);
    }
}
