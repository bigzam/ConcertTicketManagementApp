using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Tickets.Models;
using Validation;

namespace ConcertTicketManagement.Repositories.Tickets
{
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly Dictionary<Guid, List<Ticket>> _tickets = new();

        private readonly Dictionary<Guid, List<Ticket>> _shoppingCart = new();

        /// <inheritdoc/>
        public Task<Ticket?> GetAvailableTicketByIdAsync(Guid ticketId, Guid eventId, CancellationToken token)
        {

            if (!_tickets.TryGetValue(eventId, out List<Ticket>? tickets))
            {
                return Task.FromResult((Ticket?)null);
            }
            else
            {
                return Task.FromResult(
                    tickets.Where(t => t.IsAvailable && !t.IsReserved && !t.IsBlocked && t.Id == ticketId).FirstOrDefault());
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Ticket>> GetAvailableTicketsForEventAsync(Guid eventId, CancellationToken token)
        {
            if (!_tickets.TryGetValue(eventId, out List<Ticket>? tickets))
            {
                return Task.FromResult(Enumerable.Empty<Ticket>());
            }

            return Task.FromResult(
                tickets.Where(t => t.IsAvailable && !t.IsReserved && !t.IsBlocked));
        }
        /// <inheritdoc/>
        public Task<bool> ReserveAsync(Guid userId, Guid ticketId, Guid eventId, CancellationToken token)
        {
            try
            {
                var ticket = this.SetTicketReservedStatus(ticketId, eventId);

                if (ticket is null)
                {
                    return Task.FromResult(false);
                }

                if (!_shoppingCart.TryGetValue(userId, out List<Ticket>? tickets))
                {
                    _shoppingCart.Add(userId, new List<Ticket> { ticket });
                }
                else
                {
                    _shoppingCart[userId].Add(ticket);
                }
            }
            catch(InvalidOperationException)
            {
                // log exception - ticket is already reserved.
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> SetTicketsForEventAsync(IEnumerable<Ticket> tickets, CancellationToken token)
        {
            Requires.NotNull(tickets, nameof(tickets));

            var eventId = tickets.First().EventId;
            try
            {
                if (_tickets.ContainsKey(eventId))
                {
                    _tickets[eventId].AddRange(tickets);
                }
                else
                {
                    _tickets.Add(tickets.First().EventId, tickets.ToList());
                }
            }
            catch (ArgumentException)
            {
                // Log exception
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> CancelReservationAsync(Guid userId, CancellationToken token)
        {
            try
            {
                this.ReleaseTickets(userId);
            }
            catch (InvalidOperationException)
            {
                // log exception - ticket is already reserved.
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void ReleaseTickets(Guid userId)
        {
            if(_shoppingCart.TryGetValue(userId, out List<Ticket>? tickets))
            {                 
                foreach (var ticket in tickets)
                {
                    ticket.ReleaseReservation();
                }
            }
        }

        private Ticket? SetTicketReservedStatus(Guid ticketId, Guid eventId)
        {
            if (!_tickets.TryGetValue(eventId, out List<Ticket>? tickets))
            {
                return null;
            }

            var ticket = tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket != null)
            {
                ticket.MarkAsReserved();
            }

            return ticket;
        }
    }
}
