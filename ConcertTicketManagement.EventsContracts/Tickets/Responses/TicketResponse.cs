
using ConcertTicketManagement.Contracts.Tickets.Models;

namespace ConcertTicketManagement.Contracts.Tickets.Responses
{
    public sealed record TicketResponse
    {
        public Guid Id { get; set; }

        public SeatLocation SeatLocation { get; init; } = new SeatLocation();

        public decimal Price { get; init; }

        public TicketType Type { get; init; }

        public Guid EventId { get; init; }

    }
}
