using CommunityEvent.Models;

namespace CommunityEvent.Interfaces;

public interface IEventService
{
    Task<List<Event>> GetEventsAsync();
    Task<List<Event>> GetEventsAsync(DateTime date);
    Task<List<Event>> GetEventsAsync(string venueName);
    Task<List<Event>> GetEventsByActivityTypeAsync(string activityType);
    Task<List<Event>> GetUpcomingEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task AddEventAsync(Event eventItem);
    Task AddEventAsync(Event eventItem, IEnumerable<int> venueIds, IEnumerable<int> activityIds);
    Task UpdateEventAsync(Event eventItem);
    Task UpdateEventAsync(Event eventItem, IEnumerable<int> venueIds, IEnumerable<int> activityIds);
    Task DeactivateEventAsync(int id);
}
