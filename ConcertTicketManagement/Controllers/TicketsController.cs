using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [HttpGet(Name = "ViewTickets")]
        public async Task<IActionResult> ViewTicketsAsync()
        {
            await Task.Yield();
            return Ok();
        }
    }
}
