using CommunityEvent.Data;
using CommunityEvent.Interfaces;
using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Services;

public class ActivityService : IActivityService
{
    private readonly AppDbContext _context;

    public ActivityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Activity>> GetActivitiesAsync()
    {
        return await _context.Activities
            .AsNoTracking()
            .Include(activity => activity.EventActivities)
                .ThenInclude(eventActivity => eventActivity.Event)
            .OrderBy(activity => activity.ActivityName)
            .ToListAsync();
    }

    public async Task<Activity?> GetActivityByIdAsync(int id)
    {
        return await _context.Activities
            .AsNoTracking()
            .Include(activity => activity.EventActivities)
                .ThenInclude(eventActivity => eventActivity.Event)
            .FirstOrDefaultAsync(activity => activity.Id == id);
    }

    public async Task AddActivityAsync(Activity activity)
    {
        await _context.Activities.AddAsync(activity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateActivityAsync(Activity activity)
    {
        _context.Activities.Update(activity);
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateActivityAsync(int id)
    {
        var activity = await _context.Activities.FindAsync(id);

        if (activity is null)
        {
            return;
        }

        activity.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
