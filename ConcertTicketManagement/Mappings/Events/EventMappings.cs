using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Events.Responses;

namespace ConcertTicketManagement.Api.Mappings.Events
{
    public static class EventMappings
    {
        /// <summary>
        /// Maps CreateEventRequest to Event.
        /// </summary>
        /// <param name="request">CreateEventRequest object.</param>
        /// <returns>Event object.</returns>
        public static Event MapToEvent(this CreateEventRequest request)
        {
            return new Event
            {
                Id = Guid.NewGuid(),
                EventDate = request.EventDate,
                EventTime = TimeOnly.Parse(request.EventTime),
                Venue = request.Venue,
                Description = request.Description
            };
        }
        /// <summary>
        /// Maps Event to EventResponse.
        /// </summary>
        /// <param name="event">Event object.</param>
        /// <returns>EventResponse object.</returns>
        public static EventResponse MapToResponse(this Event @event)
        {
            return new EventResponse
            {
                Id = @event.Id,
                EventDate = @event.EventDate,
                EventTime = @event.EventTime,
                Venue = @event.Venue,
                Description = @event.Description
            };
        }
    }
}
