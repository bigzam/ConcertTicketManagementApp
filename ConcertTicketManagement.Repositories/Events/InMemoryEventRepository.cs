using ConcertTicketManagement.Contracts.Events.Models;

namespace ConcertTicketManagement.Repositories.Events
{
    public sealed class InMemoryEventRepository : IEventRepository
    {
        private readonly Dictionary<Guid, Event> _events = new();

        /// <inheritdoc/>
        public async Task<bool> CreateAsync(Event @event, CancellationToken token = default)
        {
            _events.Add(@event.Id, @event);

            // Simulate async operation
            await Task.CompletedTask;

            return true;
        }

        /// <inheritdoc/>
        public async Task<Event?> GetEventDetailsAsync(Guid eventId, CancellationToken token = default)
        {
            if (_events.ContainsKey(eventId))
            {
                // Simulate async operation
                await Task.CompletedTask; 
                return _events[eventId];
            }

            return null;
        }
    }
}
