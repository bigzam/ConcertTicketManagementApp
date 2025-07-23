
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Tickets.Models;
using ConcertTicketManagement.Contracts.Tickets.Requests;

namespace ConcertTicketManagement.Application.Events.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Gets Event by Id.
        /// </summary>
        /// <param name="Id">Event Id.</param>
        /// <returns>Event object if found. Null otherwise.</returns>
        Task<Event?> GetByIdAsync(Guid id, CancellationToken token);

        /// <summary>
        /// Gets All Events.
        /// </summary>
        /// <returns>List of events.</returns>
        Task<IEnumerable<Event>> GetAllAsync(CancellationToken token);

        /// <summary>
        /// Creates Event.
        /// </summary>
        /// <param name="event">Event.</param>
        /// <returns>True if event created succesfully. False otherwise.</returns>
        Task<bool> CreateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Updates Event.
        /// </summary>
        /// <param name="event">Event to update.</param>
        /// <returns>Event object if event updated succesfully. Null if event is not found.</returns>
        Task<Event?> UpdateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Sets Event tickets.
        /// </summary>
        /// <param name="tickets">Collection of tickets.</param>
        /// <returns>True if tickets created updated succesfully.</returns>
        Task<bool> SetTicketsAsync(IEnumerable<CreateTicketsRequest> ticketsRequest, Guid eventId, CancellationToken token);
    }
}
