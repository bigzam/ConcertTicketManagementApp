using ConcertTicketManagement.Application.Tickets.Services;

using Moq;
using Xunit;

namespace ConcertTicketManagement.Tests
{
    public class TicketServiceTests : IDisposable
    {
        private readonly TicketService ticketService;

        private bool disposedValue;

        public TicketServiceTests()
        {

        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

       

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }

                this.disposedValue = true;
            }
        }
    }
}
