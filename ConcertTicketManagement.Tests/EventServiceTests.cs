using ConcertTicketManagement.Application.Events.Services;

using Moq;
using Xunit;

namespace ConcertTicketManagement.Tests
{
    public class EventServiceTests : IDisposable
    {
        private readonly EventService _eventService;

        private bool disposedValue;

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
