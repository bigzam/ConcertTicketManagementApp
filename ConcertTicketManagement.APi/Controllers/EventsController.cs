using ConcertTicketManagement.Api.Auth;
using ConcertTicketManagement.Api.Mappings.Events;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Events.Responses;
using ConcertTicketManagement.Contracts.Tickets.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Gets Event by Id.
        /// </summary>
        /// <remarks>
        /// This API gets an Event by Id.
        /// </remarks>
        /// <returns>Event object.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAsync(
                        [FromRoute] string id,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator
            if (!Guid.TryParse(id, out var eventId))
            {

                return BadRequest("Invalid Event Id format. Event Id should be a valid Guid.");
            }

            var @event = await _eventService.GetByIdAsync(eventId, token);

            if (@event is null)
            {
                return NotFound();
            }

            var response = @event.MapToResponse();
            return Ok(response);
        }

        /// <summary>
        /// Creates Event.
        /// </summary>
        /// <remarks>
        /// This API creates an Event.
        /// </remarks>
        /// <returns>Event object.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(EventsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventsAsync(CancellationToken token = default)
        {
            var events = await _eventService.GetAllAsync(token);
            var response = events.MapToResponse();

            return Ok(response);
        }

        /// <summary>
        /// Creates Event.
        /// </summary>
        /// <remarks>
        /// This API creates an Event.
        /// </remarks>
        /// <returns>Event object.</returns>
        [HttpPost]
        [Authorize(AuthConstants.EventsAdminUserPolicyName)]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateEventAsync(
                        [FromBody] CreateEventRequest eventRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator and not repeated in every method
            if (!TimeOnly.TryParse(eventRequest.EventTime, out _))
            {
                return BadRequest("eventTime is not a valid time. eventTime should be a string in format hh:mm");
            }
            
            if (!DateOnly.TryParse(eventRequest.EventDate, out _))
            {
                return BadRequest("eventDate is not a valid date. Use YYYY-MM-DD format");
            }

            var @event = eventRequest.MapToEvent();
            await _eventService.CreateAsync(@event, token);

            var eventResponse = @event.MapToResponse();

            return Ok(eventResponse);
        }

        /// <summary>
        /// Updates Event.
        /// </summary>
        /// <remarks>
        /// This API updates an Event.
        /// </remarks>
        /// <returns>Updated event object.</returns>
        [HttpPut("{id}")]
        [Authorize(AuthConstants.EventsAdminUserPolicyName)]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateEventAsync(
                        [FromRoute] string id,
                        [FromBody] UpdateEventRequest eventRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator and not repeated in every method
            if (!TimeOnly.TryParse(eventRequest.EventTime, out _))
            {
                return BadRequest("eventTime is not a valid time. Valid time format: hh:mm");
            }

            if(!DateOnly.TryParse(eventRequest.EventDate, out _))
            {
                return BadRequest("eventDate is not a valid date. Valid date format: YYYY-MM-DD");
            }

            if (!Guid.TryParse(id, out var eventId))
            {

                return BadRequest("Invalid Event Id format. Event Id should be a valid Guid.");
            }

            var @event = eventRequest.MapToEvent(eventId);
            var updatedEvent = await _eventService.UpdateAsync(@event, token);

            if (updatedEvent is null)
            {
                return NotFound();
            }

            var response = updatedEvent.MapToResponse();
            return Ok(response);
        }

        /// <summary>
        /// Sets Event tickets.
        /// </summary>
        /// <remarks>
        /// This API sets tickets for an Event.
        /// </remarks>
        /// <returns>Updated event object.</returns>
        [HttpPost("{eventId}/settickets")]
        [Authorize(AuthConstants.EventsAdminUserPolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SetEventTicketsAsync(
                        [FromRoute] string eventId,
                        [FromBody] List<CreateTicketsRequest> ticketsRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator and not repeated in every method
            // TODO: Add validation for duplicate tickets, price etc.
            if (!Guid.TryParse(eventId, out var eventIdGuid))
            {

                return BadRequest("Invalid Event Id format. Event Id should be a valid Guid.");
            }

            if (ticketsRequest is null || !ticketsRequest.Any())
            {
                return BadRequest("Tickets request cannot be null or empty.");
            }

            var @event = await _eventService.GetByIdAsync(eventIdGuid, token);
            if (@event == null)
            {
                return NotFound($"Event with Id {eventId} does not exist.");
            }

            if (!await _eventService.SetTicketsAsync(ticketsRequest, eventIdGuid, token))
            {
                return StatusCode(500, "Failed to set tickets for the event.");
            }

            return Ok();
        }

        /// <summary>
        /// Blocks Event Tickets to manage capacity, staged release, scene installation etc.
        /// </summary>
        /// <param name="ticketIdList"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPatch("{eventId}/blocktickets")]
        [Authorize(AuthConstants.EventsAdminUserPolicyName)]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BlockEventTicketsAsync(
                        [FromRoute] string eventId,
                        [FromBody] List<string> ticketIdList,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator and not repeated in every method
            if (!Guid.TryParse(eventId, out var eventGuid))
            {
                return BadRequest("Invalid Event Id format. Event Id should be a valid Guid.");
            }

            if (ticketIdList is null || !ticketIdList.Any())
            {
                return BadRequest("Tickets list cannot be null or empty.");
            }

            var @event = await _eventService.GetByIdAsync(eventGuid, token);
            if (@event == null)
            {
                return NotFound($"Event with Id {eventId} not found.");
            }

            List<Guid> ticketGuids = new();
            foreach (var ticketId in ticketIdList)
            {
                if (!Guid.TryParse(ticketId, out var ticketGuid))
                {
                    return BadRequest("Invalid Ticket Id format. Ticket Id should be a valid Guid.");
                }
                ticketGuids.Add(ticketGuid);
            }
            await _eventService.BlockEventTicketsAsync(eventGuid, ticketGuids, token);

            return Ok();
        }

        [HttpPatch("{eventId}/unblocktickets")]
        [Authorize(AuthConstants.EventsAdminUserPolicyName)]
        [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnBlockEventTicketsAsync(
                [FromRoute] string eventId,
                [FromBody] List<string> ticketIdList,
                CancellationToken token = default)
        {
            // This should be done by a separate validator and not repeated in every method
            if (!Guid.TryParse(eventId, out var eventGuid))
            {
                return BadRequest("Invalid Event Id format. Event Id should be a valid Guid.");
            }

            if (ticketIdList is null || !ticketIdList.Any())
            {
                return BadRequest("Tickets list cannot be null or empty.");
            }

            List<Guid> ticketGuids = new();
            foreach (var ticketId in ticketIdList)
            {
                if (!Guid.TryParse(ticketId, out var ticketGuid))
                {
                    return BadRequest("Invalid Ticket Id format. Ticket Id should be a valid Guid.");
                }
                ticketGuids.Add(ticketGuid);
            }

            var @event = await _eventService.GetByIdAsync(eventGuid, token);
            if (@event == null)
            {
                return NotFound($"Event with Id {eventId} not found.");
            }

            await _eventService.UnBlockEventTicketsAsync(eventGuid, ticketGuids, token);

            return Ok();
        }
    }
}
