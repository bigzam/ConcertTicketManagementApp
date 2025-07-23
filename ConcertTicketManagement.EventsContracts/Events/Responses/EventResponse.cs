
namespace ConcertTicketManagement.Contracts.Events.Responses
{
    /// <summary>
    /// EventResponse class represents the response for an event.
    /// </summary>
    public sealed record EventResponse
    {
        /// <summary>
        /// Unique identifier for the event.
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        /// Date of the event.
        /// </summary>
        public required DateOnly EventDate { get; init; }

        /// <summary>
        /// Time of the event.
        /// </summary>
        public required TimeOnly EventTime { get; init; }

        /// <summary>
        /// Venue of the event.
        /// </summary>
        public required string Venue { get; init; }

        /// <summary>
        /// Description of the event.
        /// </summary>
        public required string Description { get; init; }
    }
}
