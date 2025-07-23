
namespace ConcertTicketManagement.Contracts.Tickets.Responses
{
    public sealed record TicketsResponse
    {
        public required IEnumerable<TicketResponse> Items { get; init; } = Enumerable.Empty<TicketResponse>();
    }
}
