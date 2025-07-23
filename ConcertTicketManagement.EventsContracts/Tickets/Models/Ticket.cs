
namespace ConcertTicketManagement.Contracts.Tickets.Models
{
    public sealed class Ticket
    {
        const int ReservationDurationInMinutes = 10;

        public Guid TicketId { get; private set; }

        public Guid EventId { get; private set; }

        public SeatLocation SeatLocation { get; init; } = new SeatLocation();

        public decimal Price { get; init; }

        public TicketType Type { get; init; }

        public bool IsAvailable { get; private set; } = true;

        public bool IsReserved { get; private set; } = false;

        private readonly object _ticketsLock = new();

        private DateTime? ReservationExpiresAt { get; set; } = null;

        /// <summary>
        /// Inializes a new instance of the <see cref="Ticket"/> class with specified type, price, and seat location.
        /// </summary>
        /// <param name="type">Ticket type</param>
        /// <param name="price">Ticket price</param>
        /// <param name="seatLocation">Ticket SeatLocation</param>
        public Ticket(Guid eventId, TicketType type, decimal price, SeatLocation seatLocation)
        {
            EventId = eventId;
            TicketId = Guid.NewGuid();
            Type = type;
            Price = price;
            SeatLocation = seatLocation;
        }

        /// <summary>
        /// Marks the ticket as sold, making it unavailable for further purchase.
        /// </summary>
        /// <remarks>This method ensures thread safety by locking access to the ticket's state.  If the
        /// ticket is already sold, an exception is thrown.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the ticket is already sold.</exception>
        public void MarkAsSold()
        {
            lock (_ticketsLock)
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("Ticket is already sold.");
                }

                ReservationExpiresAt = null;
                IsAvailable = false;
            }
        }

        public void MarkAsReserved()
        {
            lock (_ticketsLock)
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("Ticket is already sold.");
                }

                if (IsReserved)
                {
                    throw new InvalidOperationException("Ticket is reserved.");
                }

                ReservationExpiresAt = DateTime.UtcNow.AddMinutes(ReservationDurationInMinutes);
                IsReserved = true;
            }
        }
    }
}
