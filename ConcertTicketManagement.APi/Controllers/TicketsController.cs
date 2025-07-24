using ConcertTicketManagement.Api.Auth;
using ConcertTicketManagement.Api.Mappings.Tickets;
using ConcertTicketManagement.Application.Tickets.Services;
using ConcertTicketManagement.Contracts.Payments;
using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// View Tickets
        /// </summary>
        /// <remarks>
        /// This API retrieves available tickets for given event Id
        /// </remarks>
        /// <returns>List of available tickets.</returns>
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetAvailableTicketsAsync(string eventId, CancellationToken token)
        {
            // This should be done by a separate validator and not repeated in every method
            if (!Guid.TryParse(eventId, out var eventGuid))
            {
                return BadRequest("Invalid Event Id format. Event Id should be a valid Guid.");
            }

            var tickets = await _ticketService.GetAvailableTicketsForEventAsync(eventGuid, token);

            if (tickets is null || !tickets.Any())
            {
                return NotFound("No tickets available for the specified event.");
            }

            var ticketsResponse = tickets.MapToResponse();

            return Ok(ticketsResponse);
        }

        /// <summary>
        /// Reserves a Ticket
        /// </summary>
        /// <remarks>
        /// This API reserves a ticket by setting it's IsReserved status to true.
        /// The ticket becames unavavilable for other to reserve/buy.
        /// </remarks>
        /// <returns></returns>
        [HttpPost("{ticketId}/events/{eventId}/reservation")]
        public async Task<IActionResult> ReserveTicketAsync(string ticketId, string eventId, CancellationToken token)
        {
            // This should be done by a separate validator and not repeated in every method
            if (!Guid.TryParse(ticketId, out var ticketGuid))
            {
                return BadRequest("Invalid ticket Id format. Ticket Id should be a valid Guid.");
            }

            if (!Guid.TryParse(eventId, out var eventGuid))
            {
                return BadRequest("Invalid ticket Id format. Ticket Id should be a valid Guid.");
            }

            var ticket = _ticketService.GetAvailableTicketByIdAsync(ticketGuid, eventGuid, token);
            if (ticket is null)
            {
                return NotFound("Ticket is not found or unavavilable.");
            }

            // This requires proper authentication implementation
            // Assumption here that only authenticated users can reserve/buy tickets.
            // Can be also implemented with session Id or similar for guest users.
            var userId = HttpContext.GetUserId();

            var result = await _ticketService.ReserveAsync(userId, ticketGuid, eventGuid, token);
            if (result)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Unable to reserve ticket.");
            }
        }

        /// <summary>
        /// Releases reserved tickets and makes them available.
        /// </summary>
        /// <remarks>
        /// This API releases reserved tickets for given event Id and user Id
        /// </remarks>
        /// <returns></returns>
        [HttpDelete("reservation")]
        public async Task<IActionResult> CancelReservation(CancellationToken token)
        {
            // This requires proper authentication implementation
            // Assumption here that only authenticated users can reserve/buy tickets.
            // Can be also implemented with session Id or similar for guest users.
            var userId = HttpContext.GetUserId();

            await _ticketService.CancelReservationAsync(userId, token);

            return Ok();
        }

        /// <summary>
        /// Purches tickets currently held in the shopping cart
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PaymentMethodInformation paymentInfo, CancellationToken token)
        {
            // This requires proper authentication implementation
            // Assumption here that only authenticated users can reserve/buy tickets.
            // Can be also implemented with session Id or similar for guest users.
            var userId = HttpContext.GetUserId();

            var result = await _ticketService.PurchaseAsync(userId, paymentInfo, token);
            if (!result)
            {
                return StatusCode(500, "Unable to purchase tickets. Please check your payment information or available tickets and try again.");
            }

            return Ok();
        }
    }
}
