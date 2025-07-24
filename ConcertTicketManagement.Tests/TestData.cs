using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Payments;
using ConcertTicketManagement.Contracts.Payments.Responses;
using ConcertTicketManagement.Contracts.Tickets.Models;
using ConcertTicketManagement.Contracts.Tickets.Requests;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConcertTicketManagement.Tests
{
    // Potentially use Bogus nuget package to generate data for tests.
    public static class TestData
    {
        public static Event GetValidEvent(DateOnly eventDate, TimeOnly eventTime)
        {
            return new Event
            {
                Id = Guid.NewGuid(),
                EventDate = eventDate,
                EventTime = eventTime,
                Venue = "Test Venue",
                Description = "Test Description",
            };
        }

        public static List<CreateTicketsRequest> CreateTickets()
        {
            return new List<CreateTicketsRequest>
            {
                new CreateTicketsRequest {
                    SeatLocation = new SeatLocation { Row = "L", SeatNumber = 1 },
                    Price = 100, 
                    Type = TicketType.GeneralAdmission
                },
                new CreateTicketsRequest {
                    SeatLocation = new SeatLocation { Row = "A", SeatNumber = 10, Section = "1" },
                    Price = 100,
                    Type = TicketType.GeneralAdmission
                },
            };
        }

        public static CreateEventRequest CreateEventRequest(string eventDate, string eventTime)
        {
            return new CreateEventRequest
            {
                EventDate = eventDate,
                EventTime = eventTime,
                Venue = "Test Venue",
                Description = "Test Description"
            };
        }

        public static Ticket GetTicket(
            Guid eventId,
            int seatNumber = 10,
            decimal price = 100.00m,
            TicketType type =TicketType.GeneralAdmission)
        {
            return new Ticket
            (
                eventId,
                type,
                price,
                new SeatLocation
                {
                    Row = "R",
                    Section = "220",
                    SeatNumber = seatNumber
                }
            );
        }

        public static PaymentMethodInformation GetPaymentMethodInformation()
        {
            return new PaymentMethodInformation
            (
                "4111111111111111",
                "Test User",
                "12/25",
                "123",                
                "billingAddress"
            );
        }

        public static PaymentResponse GetSuccessfullPaymentResponse()
        {
            return new PaymentResponse
            {
                IsSuccessful = true,
                TransactionId = Guid.NewGuid().ToString(),
                TotalPrice = 200.00m,
                Tickets = new List<string> { "Ticket1", "Ticket2" }
            };
        }

        public static PaymentResponse GetFailedPaymentResponse()
        {
            return new PaymentResponse
            {
                IsSuccessful = false,
                ErrorMessage = "Payment failed",
            };
        }
    }
}
