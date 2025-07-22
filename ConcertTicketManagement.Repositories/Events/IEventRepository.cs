using ConcertTicketManagement.Contracts.Events.Models;

namespace ConcertTicketManagement.Repositories.Events
{
    public interface IEventRepository
    {
        /// <summary>
        /// Creates an event in the repository.
        /// </summary>
        /// <param name="event">Event object to be created.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if event succesfuly create. False otherwise.</returns>
        Task<bool> CreateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Gets event details from the repository.
        /// </summary>
        /// <param name="eventId">Guid representing Event Id.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Event object or null if not found representing the asynchronous operation with event details.</returns>
        Task<Event?> GetEventDetailsAsync(Guid eventId, CancellationToken token);

    }
}
