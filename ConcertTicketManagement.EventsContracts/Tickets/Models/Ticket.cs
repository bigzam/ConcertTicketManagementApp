
namespace ConcertTicketManagement.Contracts.Tickets.Models
{
    public sealed class Ticket
    {
        const int ReservationDurationInMinutes = 10;

        public Guid Id { get; private set; }

        public Guid EventId { get; private set; }

        public SeatLocation SeatLocation { get; init; } = new SeatLocation();

        public decimal Price { get; init; }

        public TicketType Type { get; init; }

        public bool IsSold { get; private set; } = false;

        public bool IsReserved { get; private set; } = false;

        /// <summary>
        /// Indicates whether the ticket is blocked for purchase - 
        /// decrease event capacity, scene installation etc
        /// </summary>
        public bool IsBlocked { get; private set; } = false;

        private readonly object _ticketsLock = new();

        /// <summary>
        /// Inializes a new instance of the <see cref="Ticket"/> class with specified type, price, and seat location.
        /// </summary>
        /// <param name="type">Ticket type</param>
        /// <param name="price">Ticket price</param>
        /// <param name="seatLocation">Ticket SeatLocation</param>
        public Ticket(Guid eventId, TicketType type, decimal price, SeatLocation seatLocation)
        {
            EventId = eventId;
            Id = Guid.NewGuid();
            Type = type;
            Price = price;
            SeatLocation = seatLocation;
        }

        /// <summary>
        /// Sets ticket status as sold, making it unavailable for further purchase.
        /// </summary>
        /// <remarks>This method ensures thread safety by locking access to the ticket's state.  If the
        /// ticket is already sold, an exception is thrown.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the ticket is already sold.</exception>
        public void SetSold()
        {
            lock (_ticketsLock)
            {
                if (IsSold)
                {
                    throw new InvalidOperationException("Ticket is already sold.");
                }

                IsSold = true;
            }
        }

        /// <summary>
        /// Sets ticket status as reserved, preventing it from being sold or reserved again.
        /// </summary>
        /// <remarks>This method sets the ticket's reservation status. It ensures
        /// that the ticket cannot be reserved if it is already sold or reserved.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the ticket is already sold or if the ticket is already reserved.</exception>
        public void SetReserved()
        {
            lock (_ticketsLock)
            {
                if (IsSold)
                {
                    throw new InvalidOperationException("Ticket is already sold.");
                }

                if (IsReserved)
                {
                    throw new InvalidOperationException("Ticket is reserved.");
                }

                IsReserved = true;
            }
        }

        /// <summary>
        /// Releases the reservation on the ticket, allowing it to be reserved or sold again.
        /// </summary>
        public void ReleaseReservation()
        {
            lock (_ticketsLock)
            {
                IsReserved = false;
            }
        }

        /// <summary>
        /// Sets ticket status as Blocked.
        /// </summary>
        public void SetBlocked()
        {
            lock (_ticketsLock)
            {
                IsBlocked = true;
            }
        }

        /// <summary>
        /// Sets ticket status as UnBlocked.
        /// </summary>
        public void SetUnBlocked()
        {
            lock (_ticketsLock)
            {
                IsBlocked = false;
            }
        }

        public void RevertSold()
        {
            lock (_ticketsLock)
            {
                IsSold = false;
            }
        }
    }
}
