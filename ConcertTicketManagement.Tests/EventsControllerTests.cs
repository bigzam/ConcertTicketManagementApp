using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Events.Responses;
using ConcertTicketManagement.Controllers;

using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConcertTicketManagement.Tests
{
    public class EventsControllerTests : IDisposable
    {
        private readonly Mock<IEventService> _eventServiceMock;

        private readonly EventsController _controller;

        private bool disposedValue;

        public EventsControllerTests()
        {
            _eventServiceMock = new Mock<IEventService>();
            _controller = new EventsController(_eventServiceMock.Object);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            var @event = TestData.GetValidEvent(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                    new TimeOnly(19, 0));

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);

            var response =
                await this._controller.GetEventAsync(@event.Id.ToString());

            var result = Assert.IsType<OkObjectResult>(response);

            EventResponse eventResponse = (EventResponse)result?.Value;

            Assert.Equal(@event.Id, eventResponse?.Id);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenEventDoesntExist()
        {
            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);

            var response =
                await this._controller.GetEventAsync(Guid.Empty.ToString());

            var result = Assert.IsType<NotFoundResult>(response);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBadRequest_WhenEventIdIsNotValidGuid()
        {
            var response =
                await this._controller.GetEventAsync("123-4456");
            
            Assert.IsType<BadRequestObjectResult>(response);
            this._eventServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenValidCreateEventRequest()
        {
            string eventTime = "18:00";
            CreateEventRequest eventRequest = TestData.CreateEventRequest(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(100)).ToString(),
                    eventTime
                );

            this._eventServiceMock.Setup(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<OkObjectResult>(response);
            EventResponse eventResponse = (EventResponse)result?.Value;

            Assert.Equal(eventRequest.Venue, eventResponse?.Venue);
            Assert.Equal(eventRequest.Description, eventResponse?.Description);
            Assert.Equal(eventRequest.EventDate, eventResponse?.EventDate.ToString());
            Assert.Equal(TimeOnly.Parse(eventTime), eventResponse?.EventTime);

            this._eventServiceMock.Verify(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("7:0")]
        [InlineData("23:59")]
        [InlineData("7:21 am")]
        [InlineData("12:45 pm")]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenEventTimeInValidFormat(string eventTime)
        {
            CreateEventRequest eventRequest = TestData.CreateEventRequest(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(100)).ToString(),
                    eventTime
                );

            this._eventServiceMock.Setup(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<OkObjectResult>(response);
            EventResponse eventResponse = (EventResponse)result?.Value;

            Assert.Equal(eventRequest.Venue, eventResponse?.Venue);
            Assert.Equal(eventRequest.Description, eventResponse?.Description);
            Assert.Equal(DateOnly.Parse(eventRequest.EventDate), eventResponse?.EventDate);
            Assert.Equal(TimeOnly.Parse(eventTime), eventResponse?.EventTime);

            this._eventServiceMock.Verify(x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("2025-10-01")]
        [InlineData("01/01/2025")]
        [InlineData("01-01-2025")]
        [InlineData("12/31/2025")]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenEventDateInValidFormat(string date)
        {
            string eventTime = "18:00";
            CreateEventRequest eventRequest = TestData.CreateEventRequest(date, eventTime);

            this._eventServiceMock.Setup(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<OkObjectResult>(response);
            EventResponse eventResponse = (EventResponse)result?.Value;

            Assert.Equal(eventRequest.Venue, eventResponse?.Venue);
            Assert.Equal(eventRequest.Description, eventResponse?.Description);
            Assert.Equal(DateOnly.Parse(eventRequest.EventDate), eventResponse?.EventDate);
            Assert.Equal(TimeOnly.Parse(eventTime), eventResponse?.EventTime);

            this._eventServiceMock.Verify(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("7")]
        [InlineData("25:99")]
        [InlineData("01:01 fm")]
        public async Task CreateEventAsync_ShouldReturnBadRequest_WhenInvalidTimeFormat(string eventTime)
        {
            CreateEventRequest eventRequest = TestData.CreateEventRequest("2026-02-10", eventTime);

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<BadRequestObjectResult>(response);
            this._eventServiceMock.Verify(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("31-31-2025")]
        [InlineData("31/31/2025")]
        public async Task CreateEventAsync_ShouldReturnBadRequest_WhenInvalidDateFormat(string eventDate)
        {
            CreateEventRequest eventRequest = TestData.CreateEventRequest(eventDate, "18:00");

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<BadRequestObjectResult>(response);
            this._eventServiceMock.Verify(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetEventsAsync_ShouldReturnAllEvents_WhenEventsExist()
        {
            Event @event1 = TestData.GetValidEvent(
                DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                new TimeOnly(19, 0));
            Event event2 =  TestData.GetValidEvent(
                DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                new TimeOnly(19, 0));

            event2.EventDate.AddMonths(1);
            event2.Description = "Another Test Description";
            event2.Venue = "Another Test Venue";

            List<Event> events = new List<Event> { event1, event2 };

            this._eventServiceMock.Setup(
                x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response =
                await this._controller.GetEventsAsync();

            var result = Assert.IsType<OkObjectResult>(response);
            EventsResponse eventsResponse = (EventsResponse)result?.Value;

            Assert.Equal(events.Count, eventsResponse?.Items.Count());
            this._eventServiceMock.Verify(
                x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetEventsAsync_ShouldReturnEmptySet_WhenNoEventsExist()
        {
            List<Event> events = new List<Event> ();

            this._eventServiceMock.Setup(
                x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);

            var response =
                await this._controller.GetEventsAsync();

            var result = Assert.IsType<OkObjectResult>(response);
            EventsResponse eventsResponse = (EventsResponse)result?.Value;

            Assert.Equal(events.Count, eventsResponse?.Items.Count());
            this._eventServiceMock.Verify(
                x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetTicketsAsync_ShouldCreateTickets_WhenEventExists()
        {
            var eventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100));
            var eventTime = new TimeOnly(20, 0);
            CreateEventRequest eventRequest = TestData.CreateEventRequest(eventDate.ToString(), eventTime.ToString());
            
            var @event = TestData.GetValidEvent(eventDate, eventTime);
            var tickets = TestData.CreateTickets();

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._eventServiceMock.Setup(
                x => x.SetTicketsAsync(tickets, @event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var response =
                await this._controller.SetEventTicketsAsync(@event.Id.ToString(), tickets, default);

            var result = Assert.IsType<OkResult>(response);

            this._eventServiceMock.Verify(x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x => x.SetTicketsAsync(tickets, @event.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetTicketsAsync_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            var tickets = TestData.CreateTickets();

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);

            var response =
                await this._controller.SetEventTicketsAsync(Guid.NewGuid().ToString(), tickets, default);

            var result = Assert.IsType<NotFoundObjectResult>(response);

            this._eventServiceMock.Verify(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(
                x => x.SetTicketsAsync(tickets, It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task BlockTicketsAsync_ShouldBlockTickets_WhenEventExists()
        {
            var eventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100));
            var eventTime = new TimeOnly(20, 0);
            
            var @event = TestData.GetValidEvent(eventDate, eventTime);
            var tickets = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._eventServiceMock.Setup(
                x => x.BlockEventTicketsAsync(@event.Id, It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(tickets));

            var response =
                await this._controller.BlockEventTicketsAsync(@event.Id.ToString(), tickets, default);

            var result = Assert.IsType<OkObjectResult>(response);

            this._eventServiceMock.Verify(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(
                x => x.BlockEventTicketsAsync(@event.Id, It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BlockTicketsAsync_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            var tickets = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);

            var response =
                await this._controller.BlockEventTicketsAsync(Guid.NewGuid().ToString(), tickets, default);

            var result = Assert.IsType<NotFoundObjectResult>(response);

            this._eventServiceMock.Verify(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x => 
                           x.BlockEventTicketsAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UnBlockTicketsAsync_ShouldUnBlockTickets_WhenEventExists()
        {
            var eventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100));
            var eventTime = new TimeOnly(20, 0);

            var @event = TestData.GetValidEvent(eventDate, eventTime);
            var tickets = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
            this._eventServiceMock.Setup(
                x => x.UnBlockEventTicketsAsync(@event.Id, It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(tickets));

            var response =
                await this._controller.UnBlockEventTicketsAsync(@event.Id.ToString(), tickets, default);

            var result = Assert.IsType<OkObjectResult>(response);

            this._eventServiceMock.Verify(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(
                x => x.UnBlockEventTicketsAsync(@event.Id, It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnBlockTicketsAsync_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            var tickets = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);

            var response =
                await this._controller.UnBlockEventTicketsAsync(Guid.NewGuid().ToString(), tickets, default);

            var result = Assert.IsType<NotFoundObjectResult>(response);

            this._eventServiceMock.Verify(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            this._eventServiceMock.Verify(x =>
                           x.UnBlockEventTicketsAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._eventServiceMock.VerifyAll();
                    this._eventServiceMock.VerifyNoOtherCalls();
                }

                this.disposedValue = true;
            }
        }
    }
}
