﻿using ConcertTicketManagement.Contracts.Tickets.Models;

namespace ConcertTicketManagement.Contracts.Tickets.Requests
{
    public sealed record CreateTicketsRequest
    {
        public required SeatLocation SeatLocation { get; init; }

        public decimal Price { get; init; }

        public TicketType Type { get; init; }
    }
}
