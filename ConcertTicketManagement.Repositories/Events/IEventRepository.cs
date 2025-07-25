﻿using ConcertTicketManagement.Contracts.Events.Models;

namespace ConcertTicketManagement.Repositories.Events
{
    public interface IEventRepository
    {
        /// <summary>
        /// Creates an event in the repository.
        /// </summary>
        /// <param name="event">Event object to be created.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if event successfully create. False otherwise.</returns>
        Task<bool> CreateAsync(Event @event, CancellationToken token);

        /// <summary>
        /// Updates an event in the repository.
        /// </summary>
        /// <param name="event">Event object to be updated.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Update event object if event successfully updated. Null otherwise.</returns>
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
