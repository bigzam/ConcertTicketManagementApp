
using ConcertTicketManagement.Contracts.Events.Models;
using ConcertTicketManagement.Repositories.Events;

namespace ConcertTicketManagement.Application.Events.Services
{
    public sealed class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateAsync(Event @event, CancellationToken token)
        {
            return await _repository.CreateAsync(@event, token);
        }

        /// <inheritdoc/>
        public async Task<Event?> GetEventDetailsAsync(Guid id, CancellationToken token)
        {
            return await _repository.GetEventDetailsAsync(id, token);
        }
    }
}
