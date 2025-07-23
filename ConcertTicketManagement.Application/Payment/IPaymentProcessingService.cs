
using ConcertTicketManagement.Contracts.Payments;

namespace ConcertTicketManagement.Application.Payment
{
    public interface IPaymentProcessingService
    {
        Task<bool> ProcessPayment(decimal paymentAmount, PaymentMethod paymentMethod);
    }
}
