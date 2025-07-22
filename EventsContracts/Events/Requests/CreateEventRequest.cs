namespace ConcertTicketManagement.Contracts.Events.Requests
{
    /// <summary>
    /// CreateEventRequest class in the concert ticket management system.
    /// </summary>
    public sealed record CreateEventRequest
    {
        /// <summary>
        /// Date of the event.
        /// </summary>
        public required DateOnly EventDate{ get; set; }

        /// <summary>
        /// Time of the event.
        /// </summary>
        public required string EventTime { get; set; }

        /// <summary>
        /// Venue of the event.
        /// </summary>
        public required string Venue { get; set; }

        /// <summary>
        /// Description of the event.
        /// </summary>
        public required string Description { get; set; }

    }
}
