using ConcertTicketManagement.Contracts.Tickets.Models;
using ConcertTicketManagement.Contracts.Tickets.Responses;

namespace ConcertTicketManagement.Api.Mappings.Tickets
{
    public static class TicketMappings
    {
        /// <summary>
        /// Maps CreateTicketsRequest to Ticket.
        /// </summary>
        /// <param name="request">CreateEventRequest object.</param>
        /// <returns>Event object.</returns>
        public static TicketResponse MapToResponse(this Ticket ticket)
        {
            return new TicketResponse
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                Type = ticket.Type,
                Price = ticket.Price,
                SeatLocation = new SeatLocation
                {
                    Section = ticket.SeatLocation.Section,
                    Row = ticket.SeatLocation.Row,
                    SeatNumber = ticket.SeatLocation.SeatNumber
                }
            };
        }

        /// <summary>
        /// Maps all Tickets to TicketssResponse.
        /// </summary>
        /// <returns>TicketsResponse object.</returns>
        public static TicketsResponse MapToResponse(this IEnumerable<Ticket> tickets)
        {
            // TODO: implement paging
            return new TicketsResponse
            {
                Items = tickets.Select(MapToResponse),
            };
        }
    }
}
