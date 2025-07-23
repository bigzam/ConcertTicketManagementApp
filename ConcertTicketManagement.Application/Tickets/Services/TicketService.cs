
using ConcertTicketManagement.Application.Payment;
using ConcertTicketManagement.Contracts.Payments;
using ConcertTicketManagement.Contracts.Tickets.Models;
using ConcertTicketManagement.Repositories.Tickets;

namespace ConcertTicketManagement.Application.Tickets.Services
{
    public sealed class TicketService : ITicketService
    {
        private IPaymentProcessingService _paymentProcessingService;
        private ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository, IPaymentProcessingService paymentProcessingService)
        {
            _ticketRepository = ticketRepository;
            _paymentProcessingService = paymentProcessingService;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Ticket>> GetAvailableTicketsForEventAsync(Guid eventId, CancellationToken token = default)
        {
            return await _ticketRepository.GetAvailableTicketsForEventAsync(eventId, token);
        }

        /// <inheritdoc/>
        public async Task<Ticket?> GetAvailableTicketByIdAsync(Guid ticketId, Guid eventId, CancellationToken token)
        {
            return await _ticketRepository.GetAvailableTicketByIdAsync(ticketId, eventId, token);
        }

        /// <inheritdoc/>
        public async Task<bool> Purchase(IEnumerable<Ticket> tickets, PaymentMethod paymentMethod, CancellationToken token)
        {
            // This whole block should be transactional.
            // All tickets should be marked as Sold or none if payment processed succesfully.
            // If ticket.MarkAsSold() fails for any ticket, payment should be rolled back.
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

        /// <inheritdoc/>
        public async Task<bool> ReserveAsync(Guid userId, Guid ticketId, Guid eventId, CancellationToken token)
        {
            return await _ticketRepository.ReserveAsync(userId, ticketId, eventId, token);
        }

        /// <inheritdoc/>
        public async Task CancelReservationAsync(Guid userId, CancellationToken token)
        {
            await _ticketRepository.CancelReservationAsync(userId, token);
        }
    }
}
