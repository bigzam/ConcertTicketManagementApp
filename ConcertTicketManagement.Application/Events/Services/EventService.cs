using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Tickets.Models;
using ConcertTicketManagement.Contracts.Tickets.Requests;
using ConcertTicketManagement.Repositories.Events;
using ConcertTicketManagement.Repositories.Tickets;

namespace ConcertTicketManagement.Application.Events.Services
{
    public sealed class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;

        public EventService(IEventRepository eventRepository, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateAsync(Event @event, CancellationToken token)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event), "Event cannot be null.");
            }

            return await _eventRepository.CreateAsync(@event, token);
        }

        /// <inheritdoc/>
        public async Task<Event?> GetByIdAsync(Guid id, CancellationToken token)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Event Id cannot be empty.", nameof(id));
            }

            return await _eventRepository.GetByIdAsync(id, token);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken token)
        {
            return await _eventRepository.GetAllAsync(token);
        }

        /// <inheritdoc/>
        public async Task<Event?> UpdateAsync(Event @event, CancellationToken token)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event), "Event cannot be null.");
            }

            return await _eventRepository.UpdateAsync(@event, token);
        }

        /// <inheritdoc/>
        public async Task<bool> SetTicketsAsync(IEnumerable<CreateTicketsRequest> ticketsRequest, Guid eventId, CancellationToken token)
        {
            if (ticketsRequest is null || !ticketsRequest.Any())
            {
                throw new ArgumentException("Tickets cannot be null or empty.", nameof(ticketsRequest));
            }

            List<Ticket> tickets = new List<Ticket>();
            foreach (var ticketRequest in ticketsRequest)
            {
                var ticket = new Ticket(
                                        eventId,
                                        ticketRequest.Type,
                                        ticketRequest.Price,
                                        ticketRequest.SeatLocation);

                tickets.Add(ticket);
            }

            return await _ticketRepository.SetTicketsForEventAsync(tickets, token);
        }
    }
}
