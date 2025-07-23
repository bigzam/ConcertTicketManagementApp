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
        /// View Ticket
        /// </summary>
        /// <remarks>
        /// This API retrieves available tickets
        /// </remarks>
        /// <returns>Available tickets.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAvailableTicketsAsync()
        {
            await Task.Yield();
            return Ok();
        }
    }
}
