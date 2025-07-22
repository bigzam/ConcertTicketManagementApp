
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;

namespace ConcertTicketManagement.Application.Events.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Creates Event.
        /// </summary>
        /// <param name="event">Concert event.</param>
        /// <returns>True if event created succesfully. False otherwise.</returns>
        Task<bool> CreateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Gets Event details.
        /// </summary>
        /// <remarks>
        /// This API returns Event details - date, venue, description.
        /// </remarks>
        /// <returns>Event if found. Null otherwise.</returns>
        Task<Event?> GetEventDetailsAsync(Guid id, CancellationToken token);
    }
}
