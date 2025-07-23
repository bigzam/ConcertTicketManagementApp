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
    public sealed class EventsControllerTests
    {
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly EventsController _controller;

        public EventsControllerTests()
        {
            _eventServiceMock = new Mock<IEventService>();
            _controller = new EventsController(_eventServiceMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            Event @event = BuildValidEvent();

            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(@event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(@event);

            var response =
                await this._controller.GetEventAsync(@event.Id.ToString());

            var result = Assert.IsType<OkObjectResult>(response);
            EventResponse eventResponse = (EventResponse)result?.Value;

            // Assert
            Assert.Equal(@event.Id, eventResponse?.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNonFound_WhenEventDoesntExist()
        {
            // Arrange
            this._eventServiceMock.Setup(
                x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);

            var response =
                await this._controller.GetEventAsync(Guid.Empty.ToString());

            // Assert
            var result = Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBadRequest_WhenEventIdIsNotValidGuid()
        {
            // Arrange
            var response =
                await this._controller.GetEventAsync("123-4456");

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task CreateEventAsync_ShouldCreateAndReturnEvent_WhenValidCreateEventRequest()
        {
            // Arrange
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

            // Assert
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
            // Arrange
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

            // Assert
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
            // Arrange
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

            // Assert
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
            // Arrange
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = "2025-10-01",
                EventTime = eventTime,
                Venue = "Test Venue",
                Description = "Test Description"
            };

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("31-31-2025")]
        [InlineData("31/31/2025")]
        public async Task CreateEventAsync_ShouldReturnBadRequest_WhenInvalidDateFormat(string date)
        {
            // Arrange
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventDate = date,
                EventTime = "18:00",
                Venue = "Test Venue",
                Description = "Test Description"
            };

            var response =
                await this._controller.CreateEventAsync(eventRequest);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(response);
        }

        private Event BuildValidEvent()
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
