
using ConcertTicketManagement.Application.Payment;
using ConcertTicketManagement.Contracts.Payments;
using ConcertTicketManagement.Contracts.Tickets.Models;

namespace ConcertTicketManagement.Application.Tickets.Services
{
    public sealed class TicketService : ITicketService
    {
        private IPaymentProcessingService _paymentProcessingService;

        public TicketService(IPaymentProcessingService paymentProcessingService)
        {
            _paymentProcessingService = paymentProcessingService;
        }
        
        public async Task<bool> Purchase(IEnumerable<Ticket> tickets, PaymentMethod paymentMethod) 
        {
            foreach (var ticket in tickets)
            {
                if (await _paymentProcessingService.ProcessPayment(ticket.Price, paymentMethod))
                {
                    ticket.MarkAsSold();
                }
                else 
                { 
                    return false;
                }
            }

            return true;
        }
    }
}
