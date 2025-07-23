using ConcertTicketManagement.Api.Mappings.Events;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Events.Responses;
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
            if (!Guid.TryParse(id, out var eventId))
            {

                return BadRequest("Invalid Event Id format. Evnt Id should be a valid Guid");
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
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEventAsync(
                        [FromRoute] Guid id,
                        [FromBody] UpdateEventRequest eventRequest,
                        CancellationToken token = default)
        {
            // This should be done by a separate validator
            if (!TimeOnly.TryParse(eventRequest.EventTime, out _))
            {
                return BadRequest("eventTime is not a valid time. ");
            }

            if(!DateOnly.TryParse(eventRequest.EventDate, out _))
            {
                return BadRequest("eventDate is not a valid date. Use YYYY-MM-DD format");
            }

            var @event = eventRequest.MapToEvent(id);
            var updatedMovie = await _eventService.UpdateAsync(@event, token);
            if (updatedMovie is null)
            {
                return NotFound();
            }

            var response = updatedMovie.MapToResponse();
            return Ok(response);
        }
    }
}
