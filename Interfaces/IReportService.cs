namespace CommunityEvent.Interfaces;

public interface IReportService
{
    Task<Dictionary<string, int>> GetEventRegistrationCountsAsync();
    Task<Dictionary<string, int>> GetVenueUsageCountsAsync();
    Task<Dictionary<string, int>> GetActivityPopularityCountsAsync();
}
