
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;

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
        /// Gets Event details.
        /// </summary>
        /// <remarks>
        /// This API returns Event details - date, venue, description.
        /// </remarks>
        /// <returns>Event if found. Null otherwise.</returns>
        Task<Event?> GetEventDetailsAsync(Guid id, CancellationToken token);
    }
}
