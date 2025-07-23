
namespace ConcertTicketManagement.Contracts.Tickets.Models
{
    public sealed record SeatLocation
    {
        public string Section { get; init; } = string.Empty;

        public string Row { get; init; } = string.Empty;

        public int SeatNumber { get; init; }
    }
}
