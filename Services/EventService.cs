using CommunityEvent.Data;
using CommunityEvent.Exceptions;
using CommunityEvent.Interfaces;
using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetEventsAsync()
    {
        return await BaseEventQuery()
            .OrderBy(eventItem => eventItem.EventDate)
            .ToListAsync();
    }

    public async Task<List<Event>> GetEventsAsync(DateTime date)
    {
        return await BaseEventQuery()
            .Where(eventItem => eventItem.EventDate.Date == date.Date)
            .OrderBy(eventItem => eventItem.StartTime)
            .ToListAsync();
    }

    public async Task<List<Event>> GetEventsAsync(string venueName)
    {
        return await BaseEventQuery()
            .Where(eventItem => eventItem.EventVenues
                .Any(eventVenue => eventVenue.Venue != null &&
                                   eventVenue.Venue.VenueName.Contains(venueName)))
            .OrderBy(eventItem => eventItem.EventDate)
            .ToListAsync();
    }

    public async Task<List<Event>> GetUpcomingEventsAsync()
    {
        return await BaseEventQuery()
            .Where(eventItem => eventItem.IsActive && eventItem.EventDate.Date >= DateTime.Today)
            .OrderBy(eventItem => eventItem.EventDate)
            .ThenBy(eventItem => eventItem.StartTime)
            .ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await BaseEventQuery()
            .FirstOrDefaultAsync(eventItem => eventItem.Id == id);
    }

    public async Task AddEventAsync(Event eventItem)
    {
        ValidateEvent(eventItem);

        await _context.Events.AddAsync(eventItem);
        await _context.SaveChangesAsync();
    }

    public async Task AddEventAsync(Event eventItem, IEnumerable<int> venueIds, IEnumerable<int> activityIds)
    {
        ValidateEvent(eventItem);

        var selectedVenueIds = venueIds.ToHashSet();
        var selectedActivityIds = activityIds.ToHashSet();

        foreach (var venueId in selectedVenueIds)
        {
            eventItem.EventVenues.Add(new EventVenue { Event = eventItem, VenueId = venueId });
        }

        foreach (var activityId in selectedActivityIds)
        {
            eventItem.EventActivities.Add(new EventActivity { Event = eventItem, ActivityId = activityId });
        }

        await _context.Events.AddAsync(eventItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(Event eventItem)
    {
        ValidateEvent(eventItem);

        eventItem.UpdatedAt = DateTime.UtcNow;
        _context.Events.Update(eventItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateEventAsync(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem is null)
        {
            throw new EventNotFoundException(id);
        }

        eventItem.IsActive = false;
        eventItem.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private IQueryable<Event> BaseEventQuery()
    {
        return _context.Events
            .AsNoTracking()
            .Include(eventItem => eventItem.Registrations)
                .ThenInclude(registration => registration.Participant)
            .Include(eventItem => eventItem.EventVenues)
                .ThenInclude(eventVenue => eventVenue.Venue)
            .Include(eventItem => eventItem.EventActivities)
                .ThenInclude(eventActivity => eventActivity.Activity);
    }

    private static void ValidateEvent(Event eventItem)
    {
        if (eventItem.EventDate.Date < DateTime.Today)
        {
            throw new InvalidEventDateException();
        }

        if (eventItem.StartTime >= eventItem.EndTime)
        {
            throw new ArgumentException("Start time must be before end time.");
        }
    }
}
