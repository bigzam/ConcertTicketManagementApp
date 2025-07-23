using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Contracts.Events.Requests;
using ConcertTicketManagement.Contracts.Events.Responses;
using ConcertTicketManagement.Controllers;
using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;

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
            Event @event = BuildValidEvent();

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);

            var response =
                await this._controller.GetEventAsync(@event.Id.ToString());

            var result = Assert.IsType<OkObjectResult>(response);
            EventResponse eventResponse = (EventResponse)result?.Value;

            Assert.Equal(@event.Id, eventResponse?.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenEventDoesntExist()
        {
            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);

            var response =
                await this._controller.GetEventAsync(Guid.Empty.ToString());

            var result = Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBadRequest_WhenEventIdIsNotValidGuid()
        {
            var response =
                await this._controller.GetEventAsync("123-4456");
            
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenValidCreateEventRequest()
        {
            string eventTime = "18:00";
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100)).ToString(),
                EventTime = eventTime,
                Venue = "Test Venue",
                Description = "Test Description"
            };

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
        }

        [Theory]
        [InlineData("7:0")]
        [InlineData("23:59")]
        [InlineData("7:21 am")]
        [InlineData("12:45 pm")]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenEventTimeInValidFormat(string time)
        {
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100)).ToString(),
                EventTime = time,
                Venue = "Test Venue",
                Description = "Test Description"
            };

            this._eventServiceMock.Setup(
                x => x.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<OkObjectResult>(response);
            EventResponse eventResponse = (EventResponse)result?.Value;

            Assert.Equal(eventRequest.Venue, eventResponse?.Venue);
            Assert.Equal(eventRequest.Description, eventResponse?.Description);
            Assert.Equal(DateOnly.Parse(eventRequest.EventDate), eventResponse?.EventDate);
            Assert.Equal(TimeOnly.Parse(time), eventResponse?.EventTime);
        }

        [Theory]
        [InlineData("2025-10-01")]
        [InlineData("01/01/2025")]
        [InlineData("01-01-2025")]
        [InlineData("12/31/2025")]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenEventDateInValidFormat(string date)
        {
            string eventTime = "18:00";
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = date,
                EventTime = eventTime,
                Venue = "Test Venue",
                Description = "Test Description"
            };

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
        }

        [Theory]
        [InlineData("7")]
        [InlineData("25:99")]
        [InlineData("01:01 fm")]
        public async Task CreateEventAsync_ShouldReturnBadRequest_WhenInvalidTimeFormat(string eventTime)
        {
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = "2025-10-01",
                EventTime = eventTime,
                Venue = "Test Venue",
                Description = "Test Description"
            };

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("31-31-2025")]
        [InlineData("31/31/2025")]
        public async Task CreateEventAsync_ShouldReturnBadRequest_WhenInvalidDateFormat(string date)
        {
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = date,
                EventTime = "18:00",
                Venue = "Test Venue",
                Description = "Test Description"
            };

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            var result = Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetEventsAsync_ShouldReturnAllEvents_WhenEventsExist()
        {
            Event event1 = BuildValidEvent();
            Event event2 = BuildValidEvent();
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

        // Potentially use Bogus nuget package to generate data for tests.
        private static Event BuildValidEvent()
        {
            return new Event
            {
                Id = Guid.NewGuid(),
                EventDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100)),
                EventTime = new TimeOnly(18, 0),
                Venue = "Test Venue",
                Description = "Test Description"
            };
        }
    }
}
