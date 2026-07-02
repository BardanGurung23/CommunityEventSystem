using CommunityEvent.Models;

namespace CommunityEvent.Interfaces;

public interface IActivityService
{
    Task<List<Activity>> GetActivitiesAsync();
    Task<Activity?> GetActivityByIdAsync(int id);
    Task AddActivityAsync(Activity activity);
    Task UpdateActivityAsync(Activity activity);
    Task DeactivateActivityAsync(int id);
}
