namespace ConcertTicketManagement.Contracts.Events.Requests
{
    /// <summary>
    /// UpdateEventRequest class in the concert ticket management system.
    /// </summary>
    public sealed record UpdateEventRequest
    {
        /// <summary>
        /// Id of the event.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Date of the event.
        /// </summary>
        public required string EventDate{ get; set; }

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
