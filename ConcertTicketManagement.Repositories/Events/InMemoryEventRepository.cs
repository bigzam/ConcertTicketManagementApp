using ConcertTicketManagement.Contracts.Events.Models;

namespace ConcertTicketManagement.Repositories.Events
{
    public sealed class InMemoryEventRepository : IEventRepository
    {
        private readonly Dictionary<Guid, Event> _events = new();

        /// <inheritdoc/>
        public async Task<bool> CreateAsync(Event @event, CancellationToken token = default)
        {
            var result = _events.TryAdd(@event.Id, @event);

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken token)
        {
            await Task.CompletedTask;

            return _events.Values.ToList();
        }

        /// <inheritdoc/>
        public async Task<Event?> GetByIdAsync(Guid eventId, CancellationToken token = default)
        {
            if (_events.ContainsKey(eventId))
            {
                // Simulate async operation
                await Task.CompletedTask; 
                return _events[eventId];
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<Event?> UpdateAsync(Event @event, CancellationToken token)
        {
            if (_events.ContainsKey(@event.Id))
            {
                await Task.CompletedTask;
                _events[@event.Id] = @event;

                return _events[@event.Id];
            }

            return null;
        }
    }
}
