using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets Event details.
        /// </summary>
        /// <remarks>
        /// This API returns Event details - date, venue, description.
        /// </remarks>
        /// <returns>Available tickets.</returns>
        [HttpGet(Name = "GetEventDetails")]
        public async Task<IActionResult> GetEventDetailsAsync()
        {
            await Task.Yield();
            return Ok();
        }
    }
}
