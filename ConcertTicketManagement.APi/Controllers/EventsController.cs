using ConcertTicketManagement.Api.Mappings.Events;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Tickets.Requests;
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
        //[ProducesResponseType(typeof(EventsResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest)]
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
        public async Task<IActionResult> CreateEventAsync(
                        [FromBody] CreateEventRequest eventRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator
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

            return new OkObjectResult(eventResponse);
        }

        /// <summary>
        /// Updates Event.
        /// </summary>
        /// <remarks>
        /// This API updates an Event.
        /// </remarks>
        /// <returns>Updated event object.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventAsync(
                        [FromRoute] string id,
                        [FromBody] UpdateEventRequest eventRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator
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
        public async Task<IActionResult> SetEventTicketsAsync(
                        [FromRoute] string eventId,
                        [FromBody] List<CreateTicketsRequest> ticketsRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator
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
                return BadRequest("Failed to set tickets for the event.");
            }

            return Ok();
        }
    }
}
