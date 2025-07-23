using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;

        public TicketsController(ILogger<EventsController> logger)
        {
            _logger = logger;
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
            await Task.Yield();
            return Ok();
        }
    }
}
