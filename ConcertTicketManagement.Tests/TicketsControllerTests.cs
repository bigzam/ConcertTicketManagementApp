using System.Security.Claims;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Application.Tickets.Services;
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Payments.Responses;
using ConcertTicketManagement.Contracts.Tickets.Models;
using ConcertTicketManagement.Contracts.Tickets.Responses;
using ConcertTicketManagement.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace ConcertTicketManagement.Tests
{
    public class TicketsControllerTests : IDisposable
    {
        private readonly Mock<ITicketService> _ticketServiceMock;
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly Mock<IMemoryCache> _cacheMock;

        private Mock<HttpContext> _mockHttpContext = new Mock<HttpContext>();
        private readonly TicketsController _controller;

        private bool disposedValue;

        public TicketsControllerTests()
        {
            _ticketServiceMock = new Mock<ITicketService>();
            _eventServiceMock = new Mock<IEventService>();
            _cacheMock = new Mock<IMemoryCache>();
            _controller = new TicketsController(
                _ticketServiceMock.Object,
                _eventServiceMock.Object,
                _cacheMock.Object);

            _mockHttpContext = new Mock<HttpContext>();

            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();

            mockClaimsPrincipal.Setup(cp => cp.Claims)
            .Returns(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "ad3356e4-57df-4ca3-b102-0f07c50d14af")
            });

            // Setup HttpContext to return the mocked ClaimsPrincipal
            _mockHttpContext.Setup(ctx => ctx.User)
                .Returns(mockClaimsPrincipal.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object
            };
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetAvailableTickets_ShouldReturnOnlyAvailableTickets_WhenEventAndTicketsExist()
        {
            Event @event = TestData.GetValidEvent(
                 DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                 new TimeOnly(19, 0));

            var tickets = new List<Ticket>() { TestData.GetTicket(@event.Id) };

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._ticketServiceMock.Setup(
                x => x.GetAvailableTicketsForEventAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tickets);

            var response =
                await this._controller.GetAvailableTicketsAsync(@event.Id.ToString(), default);

            var result = Assert.IsType<OkObjectResult>(response);
            var ticketResponse = (TicketsResponse)result?.Value;

            Assert.Equal(1, ticketResponse?.Items.Count());
            Assert.Equal(@event.Id, ticketResponse?.Items.First().EventId);

            this._ticketServiceMock.Verify(x => x.GetAvailableTicketsForEventAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAvailableTickets_ShouldReturnNotFound_WhenTicketsSoldOut()
        {
            Event @event = TestData.GetValidEvent(
                  DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                  new TimeOnly(19, 0));

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._ticketServiceMock.Setup(
                x => x.GetAvailableTicketsForEventAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Ticket>());

            var response =
                await this._controller.GetAvailableTicketsAsync(@event.Id.ToString(), default);

            var result = Assert.IsType<NotFoundObjectResult>(response);

            this._ticketServiceMock.Verify(x => x.GetAvailableTicketsForEventAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReserveTicket_ShouldSucceed_WhenEventAndTicketExist()
        {
            Event @event = TestData.GetValidEvent(
                 DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                 new TimeOnly(19, 0));

            var ticket = TestData.GetTicket(@event.Id);
            this.SetupCahceMock(ticket);

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._ticketServiceMock.Setup(
                x => x.GetAvailableTicketByIdAsync(ticket.Id, @event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);

            this._ticketServiceMock.Setup(
                x => x.ReserveAsync(It.IsAny<Guid>(), ticket.Id, @event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var response =
                await this._controller.ReserveTicketAsync(ticket.Id.ToString(), @event.Id.ToString(), default);

            var result = Assert.IsType<OkResult>(response);

            this._ticketServiceMock.Verify(x => x.GetAvailableTicketByIdAsync(ticket.Id, @event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReserveTicket_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            Event @event = TestData.GetValidEvent(
                  DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                  new TimeOnly(19, 0));

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._ticketServiceMock.Setup(
                x => x.GetAvailableTicketByIdAsync(It.IsAny<Guid>(), @event.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Ticket?)null);

            var response =
                await this._controller.ReserveTicketAsync(Guid.NewGuid().ToString(), @event.Id.ToString(), default);

            var result = Assert.IsType<NotFoundObjectResult>(response);

            this._ticketServiceMock.Verify(x => x.GetAvailableTicketByIdAsync(It.IsAny<Guid>(), @event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PurchaseTickets_ShouldSucceed_WhenEventAndTicketExist()
        {
            var paymentMethodInformation = TestData.GetPaymentMethodInformation();
            var paymentResponse = TestData.GetSuccessfulPaymentResponse();
            var tickets = new List<Ticket>() { TestData.GetTicket(Guid.NewGuid()) };

            this.SetupCahceMock(tickets.First());

            this._ticketServiceMock.Setup(
                x => x.PurchaseAsync(
                    It.IsAny<Guid>(),
                    tickets,
                    paymentMethodInformation,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentResponse);

            var response =
                await this._controller.Purchase(paymentMethodInformation, default);

            var result = Assert.IsType<OkObjectResult>(response);
            PaymentResponse actualResponse = (PaymentResponse)result.Value;

            Assert.True(actualResponse.IsSuccessful);

            this._ticketServiceMock.Verify(
                x => x.PurchaseAsync(
                    It.IsAny<Guid>(),
                    tickets,
                    paymentMethodInformation,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PurchaseTicket_ShouldFail_WhenPaymentFails()
        {
            var paymentMethodInformation = TestData.GetPaymentMethodInformation();
            var paymentResponse = TestData.GetFailedPaymentResponse();
            var tickets = new List<Ticket>() { TestData.GetTicket(Guid.NewGuid()) };

            this.SetupCahceMock(tickets.First());
            this._ticketServiceMock.Setup(
                x => x.PurchaseAsync(
                    It.IsAny<Guid>(),
                    tickets,
                    paymentMethodInformation, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentResponse);

            var response =
                await this._controller.Purchase(paymentMethodInformation, default);

            var result = Assert.IsType<ObjectResult>(response);
            string actualResponse = (string)result.Value;

            Assert.True(actualResponse.Contains("Payment failed", StringComparison.InvariantCultureIgnoreCase));

            this._ticketServiceMock.Verify(
                x => x.PurchaseAsync(
                    It.IsAny<Guid>(),
                    tickets,
                    paymentMethodInformation,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private void SetupCahceMock(Ticket ticket)
        {
            ICacheEntry? capturedEntry = null;
            object? capturedValue = null;

            object? cachedValue = new List<Ticket> { ticket };

            this._cacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out cachedValue))
                .Returns(true);
            this._cacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns((object key) =>
                {
                    var entry = new Mock<ICacheEntry>();
                    entry.SetupSet(e => e.Value = It.IsAny<object>())
                         .Callback<object>(val => capturedValue = val);
                    entry.SetupGet(e => e.Key).Returns(key);
                    capturedEntry = entry.Object;
                    return capturedEntry;
                });
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._ticketServiceMock.VerifyAll();
                    this._ticketServiceMock.VerifyNoOtherCalls();

                    this._eventServiceMock.VerifyAll();
                    this._eventServiceMock.VerifyNoOtherCalls();
                }

                this.disposedValue = true;
            }
        }
    }
}
