using ConcertTicketManagement.Application.Payment;
using ConcertTicketManagement.Contracts.Payments;
using ConcertTicketManagement.Contracts.Payments.Responses;
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
        public async Task<PaymentResponse> PurchaseAsync(Guid userId, PaymentMethodInformation paymentMethodInformation, CancellationToken token)
        {
            var tickets = await _ticketRepository.GetTicketsFromShoppingCart(userId, token);
            if (tickets == null || !tickets.Any())
            {
                return new PaymentResponse { ErrorMessage = "Shopping card is empty!" };
            }

            // This whole block should be transactional.
            // All tickets should be marked as Sold or none if payment processed successfully.
            // If ticket.MarkAsSold() fails for any ticket, payment should be rolled back.
            var totalPrice = tickets.Sum(t => t.Price);
            var paymentId = await _paymentProcessingService.ProcessPayment(totalPrice, paymentMethodInformation);
            var soldTickets = new List<Ticket>();

            foreach (var ticket in tickets)
            {
                try
                {
                    ticket.SetSold();
                    soldTickets.Add(ticket);

                    await _ticketRepository.CancelReservationAsync(userId, token);
                }
                catch (InvalidOperationException ex)
                {
                    await _paymentProcessingService.RevertPayment(paymentId);

                    // Revert tickets sale
                    foreach (var soldTicket in soldTickets)
                    {
                        soldTicket.RevertSold();
                    }
                    return new PaymentResponse { ErrorMessage = ex.Message };
                }
            }

            return new PaymentResponse
            {
                IsSuccessful = true,
                TransactionId = paymentId,
                TotalPrice = totalPrice,
                Tickets = soldTickets.Select(t => t.Id.ToString()),
            };
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
