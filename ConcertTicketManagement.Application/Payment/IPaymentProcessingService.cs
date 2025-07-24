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
        Task<string> ProcessPayment(decimal paymentAmount, PaymentMethodInformation paymentMethod);

        /// <summary>
        /// Reverts payment is case of failure or cancellation.
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        Task<bool> RevertPayment(string paymentId);
    }
}
