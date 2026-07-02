using CommunityEvent.Models;

namespace CommunityEvent.Interfaces;

public interface IVenueService
{
    Task<List<Venue>> GetVenuesAsync();
    Task<Venue?> GetVenueByIdAsync(int id);
    Task AddVenueAsync(Venue venue);
    Task UpdateVenueAsync(Venue venue);
    Task SetVenueAvailabilityAsync(int id, bool isAvailable);
}
