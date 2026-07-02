using CommunityEvent.Data;
using CommunityEvent.Interfaces;
using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, int>> GetEventRegistrationCountsAsync()
    {
        return await _context.Events
            .AsNoTracking()
            .Select(eventItem => new
            {
                eventItem.EventName,
                Count = eventItem.Registrations.Count(registration => registration.Status != RegistrationStatus.Cancelled)
            })
            .ToDictionaryAsync(item => item.EventName, item => item.Count);
    }

    public async Task<Dictionary<string, int>> GetVenueUsageCountsAsync()
    {
        return await _context.EventVenues
            .AsNoTracking()
            .Where(eventVenue => eventVenue.Venue != null)
            .GroupBy(eventVenue => eventVenue.Venue!.VenueName)
            .ToDictionaryAsync(group => group.Key, group => group.Count());
    }

    public async Task<Dictionary<string, int>> GetActivityPopularityCountsAsync()
    {
        return await _context.EventActivities
            .AsNoTracking()
            .Where(eventActivity => eventActivity.Activity != null)
            .GroupBy(eventActivity => eventActivity.Activity!.ActivityName)
            .ToDictionaryAsync(group => group.Key, group => group.Count());
    }
}
