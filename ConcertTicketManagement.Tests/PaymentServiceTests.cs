using ConcertTicketManagement.Application.Payment;

using Moq;
using Xunit;

namespace ConcertTicketManagement.Tests
{
    public class PaymentServiceTests : IDisposable
    {
        private readonly PaymentProcessingService _paymentService;

        private bool disposedValue;

        public PaymentServiceTests()
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
