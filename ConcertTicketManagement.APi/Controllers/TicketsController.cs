using ConcertTicketManagement.Api.Auth;
using ConcertTicketManagement.Api.Mappings.Tickets;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Application.Tickets.Services;
using ConcertTicketManagement.Contracts.Payments;
using ConcertTicketManagement.Contracts.Tickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        private readonly IEventService _eventService;

        private readonly IMemoryCache _cache;

        // TODO: move to appsettings
        private readonly double ShoppingCartHoldInSeconds = 300;

        public TicketsController(ITicketService ticketService, IEventService eventService, IMemoryCache cache)
        {
            _ticketService = ticketService;
            _eventService = eventService;
            _cache = cache;
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

            var @event = await _eventService.GetByIdAsync(eventGuid, token);
            if (@event == null)
            {
                return NotFound($"Event with Id {eventId} not found.");
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
        /// The ticket becomes unavailable for other to reserve/buy.
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

            var @event = await _eventService.GetByIdAsync(eventGuid, token);
            if (@event == null)
            {
                return NotFound($"Event with Id {eventId} not found.");
            }

            var ticket = await _ticketService.GetAvailableTicketByIdAsync(ticketGuid, eventGuid, token);
            if (ticket is null)
            {
                return NotFound("Ticket is not found or unavailable.");
            }

            // This requires proper authentication implementation
            // Assumption here that only authenticated users can reserve/buy tickets.
            // Can be also implemented with session Id, user IP or similar for guest users.
            var userId = HttpContext.GetUserId();

            var result = await _ticketService.ReserveAsync(userId, ticketGuid, eventGuid, token);
            if (result)
            {
                var cacheItems = _cache.GetOrCreate(userId, entry => new List<Ticket>());
                cacheItems!.Add(ticket);
                _cache.Set(userId, cacheItems, TimeSpan.FromSeconds(ShoppingCartHoldInSeconds));

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
            // Can be also implemented with session Id, user IP or similar for guest users.
            var userId = HttpContext.GetUserId();
            var tickets = _cache.GetOrCreate(userId, entry => new List<Ticket>());
            
            if(tickets is not null && tickets.Count != 0)
            {
                await _ticketService.CancelReservationAsync(tickets, token);
                _cache.Remove(userId);
            }


            return Ok();
        }

        /// <summary>
        /// Purchases tickets currently held in the shopping cart
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PaymentMethodInformation paymentInfo, CancellationToken token)
        {
            // This requires proper authentication implementation
            // Assumption here that only authenticated users can reserve/buy tickets.
            // Can be also implemented with session Id, user IP or similar for guest users.
            var userId = HttpContext.GetUserId();

            var shopingCartTickets = _cache.GetOrCreate(userId, entry => new List<Ticket>());
            if(shopingCartTickets is null || shopingCartTickets.Count == 0)
            {
                return NotFound("The shopping cart is empty");
            }

            var paymentResult = await _ticketService.PurchaseAsync(userId, shopingCartTickets, paymentInfo, token);
           
            if (!paymentResult.IsSuccessful)
            {
                return StatusCode(500, $"Unable to purchase tickets. {paymentResult.ErrorMessage}");
            }

            _cache.Remove(userId);

            return Ok(paymentResult);
        }
    }
}
