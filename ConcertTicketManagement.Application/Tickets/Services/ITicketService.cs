﻿using System.Collections.Generic;
using ConcertTicketManagement.Contracts.Payments;
using ConcertTicketManagement.Contracts.Payments.Responses;
using ConcertTicketManagement.Contracts.Tickets.Models;

namespace ConcertTicketManagement.Application.Tickets.Services
{
    public interface ITicketService
    {
        /// <summary>
        /// Gets available tickets for a specific event.
        /// </summary>
        /// <param name="eventGuid">EVent Id</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<IEnumerable<Ticket>> GetAvailableTicketsForEventAsync(Guid eventId, CancellationToken token);

        /// <summary>
        /// Gets ticket by Id
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Ticket</returns>
        Task<Ticket?> GetAvailableTicketByIdAsync(Guid ticketId, Guid eventId, CancellationToken token);

        /// <summary>
        /// Sets ticket Reserved status to true.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ticketId"></param>
        /// <param name="token"></param>
        /// <returns>True if Reserved status set to true.</returns>
        Task<bool> ReserveAsync(Guid userId, Guid ticketId, Guid eventId, CancellationToken token);

        /// <summary>
        /// Purchases tickets for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tickets">Tickets to purchase</param>
        /// <param name="paymentMethodInformation"></param>
        /// <param name="token"></param>
        /// <returns>PaymentResponse</returns>
        Task<PaymentResponse> PurchaseAsync(Guid userId, IEnumerable<Ticket> tickets, PaymentMethodInformation paymentMethodInformation, CancellationToken token);

        /// <summary>
        /// Cancels reservation for the tickets.
        /// </summary>
        /// <param name="tickets"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task CancelReservationAsync(IEnumerable<Ticket> tickets, CancellationToken token);
    }
}
