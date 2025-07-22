using ConcertTicketManagement.Api.Mappings.Events;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;

using Microsoft.AspNetCore.Mvc;


namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IEventService _eventService;

        public EventsController(ILogger<EventsController> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        /// <summary>
        /// Gets Event details.
        /// </summary>
        /// <remarks>
        /// This API returns Event details - date, venue, description.
        /// </remarks>
        /// <returns>Available tickets.</returns>
        [HttpGet("{{id:guid}}")]
        public async Task<IActionResult> GetEventDetailsAsync([FromRoute] Guid id, CancellationToken token)
        {
            var @event = await _eventService.GetEventDetailsAsync(id, token);
            if (@event is null)
            {
                return NotFound();
            }

            return new OkObjectResult(@event);
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
                        [FromBody]CreateEventRequest eventRequest, 
                        CancellationToken token = default)
        {

            var @event = eventRequest.MapToEvent();
            await _eventService.CreateAsync(@event, token);
            
            var eventResponse = @event.MapToResponse();

            return new OkObjectResult(eventResponse);
        }
    }
}
