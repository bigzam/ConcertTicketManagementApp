using ConcertTicketManagement.Contracts.Tickets.Models;

namespace ConcertTicketManagement.Repositories.Tickets
{
    public interface ITicketRepository
    {
        /// <summary>
        /// Gets available tickets for a specific event.
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Collection of tickets</returns>
        Task<IEnumerable<Ticket>> GetAvailableTicketsForEventAsync(Guid eventId, CancellationToken token);

        /// <summary>
        /// Sets tickets for the event.
        /// </summary>
        /// <param name="tickets">Tickets to be created.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if event succesfuly create. False otherwise.</returns>
        Task<bool> SetTicketsForEventAsync(IEnumerable<Ticket> tickets, CancellationToken token);

        /// <summary>
        /// Gets ticket by eventId and ticketId
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="eventId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Ticket?> GetAvailableTicketByIdAsync(Guid ticketId, Guid eventId, CancellationToken token);

        /// <summary>
        /// Reserves a ticket by setting its IsReserved status to true and adding it to the shopping cart.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ticketId"></param>
        /// <param name="eventId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> ReserveAsync(Guid userId, Guid ticketId, Guid eventId, CancellationToken token);

        /// <summary>
        /// Cancels reservation by setting IsReserved status to false and removing it from the shopping cart.
        /// </summary>
        /// <param name="userId"></param>

        /// <param name="token"></param>
        /// <returns>True if succesfull</returns>
        Task<bool> CancelReservationAsync(Guid userId, CancellationToken token);
    }
}
