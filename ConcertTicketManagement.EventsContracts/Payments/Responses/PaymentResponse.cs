namespace ConcertTicketManagement.Contracts.Payments.Responses
{
    public sealed record PaymentResponse
    {
        public bool IsSuccessful { get; init; } = false;

        public string? ErrorMessage { get; init; }

        public string? TransactionId { get; init; }

        public decimal TotalPrice { get; init; }

        public IEnumerable<string> Tickets { get; init; } = Enumerable.Empty<string>();
    }
}
