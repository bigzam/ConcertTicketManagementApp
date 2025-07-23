
namespace ConcertTicketManagement.Contracts.Events.Models
{
    /// <summary>
    /// Event class represents an event in the concert ticket management system.
    /// </summary>
    public sealed record Event
    {
        public required Guid Id { get; set; }

        /// <summary>
        /// Date of the event.
        /// </summary>
        public required DateOnly EventDate { get; set; }

        /// <summary>
        /// Time of the event.
        /// </summary>
        public required TimeOnly EventTime { get; set; }

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
