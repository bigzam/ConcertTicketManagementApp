using ConcertTicketManagement.Contracts.Events.Models;

namespace ConcertTicketManagement.Repositories.Tickets
{
    public interface ITicketRepository
    {
        /// <summary>
        /// Creates an event in the repository.
        /// </summary>
        /// <param name="event">Event object to be created.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if event succesfuly create. False otherwise.</returns>
        Task<bool> CreateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Updates an event in the repository.
        /// </summary>
        /// <param name="event">Event object to be updated.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Update enevt object if event succesfuly updated. Null otherwise.</returns>
        Task<Event?> UpdateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Gets event details from the repository.
        /// </summary>
        /// <param name="eventId">Guid representing Event Id.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Event object or null if not found representing the asynchronous operation with event details.</returns>
        Task<Event?> GetByIdAsync(Guid eventId, CancellationToken token);

        /// <summary>
        /// Gets all events.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Collection of all Events.</returns>
        Task<IEnumerable<Event>> GetAllAsync(CancellationToken token);

    }
}
