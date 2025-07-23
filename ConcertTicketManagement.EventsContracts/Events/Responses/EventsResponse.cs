
namespace ConcertTicketManagement.Contracts.Events.Responses
{
    /// <summary>
    /// EventsResponse class represents the response for all events.
    /// </summary>
    public sealed record EventsResponse
    {
        /// <summary>
        /// Collection of all events.
        /// </summary>
        public required IEnumerable<EventResponse> Items { get; init; } = Enumerable.Empty<EventResponse>();
    }
}
