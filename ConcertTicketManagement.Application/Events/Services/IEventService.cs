
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
        /// <returns>True if event created successfully. False otherwise.</returns>
        Task<bool> CreateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Updates Event.
        /// </summary>
        /// <param name="event">Event to update.</param>
        /// <returns>Event object if event updated successfully. Null if event is not found.</returns>
        Task<Event?> UpdateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Sets Event tickets.
        /// </summary>
        /// <param name="tickets">Collection of tickets.</param>
        /// <returns>True if tickets created successfully.</returns>
        Task<bool> SetTicketsAsync(IEnumerable<CreateTicketsRequest> ticketsRequest, Guid eventId, CancellationToken token);

        /// <summary>
        /// Blocks Event Tickets to manage capacity, staged release, scene installation etc.
        /// </summary>
        /// <param name="eventGuid"></param>
        /// <param name="ticketIdList"></param>
        /// <param name="token"></param>
        /// <returns>List of unblocked ticket Ids</returns>
        Task<List<string>> BlockEventTicketsAsync(Guid eventGuid, IEnumerable<Guid> ticketIdList, CancellationToken token);

        /// <summary>
        /// Unblock tickets for the event and make them available for purchase.
        /// </summary>
        /// <param name="eventGuid"></param>
        /// <param name="ticketIdList"></param>
        /// <param name="token"></param>
        /// <returns>List of unblocked ticket Ids</returns>
        Task<List<string>> UnBlockEventTicketsAsync(Guid eventGuid, IEnumerable<Guid> ticketIdList, CancellationToken token);
    }
}
