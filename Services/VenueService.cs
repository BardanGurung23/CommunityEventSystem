using CommunityEvent.Data;
using CommunityEvent.Interfaces;
using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Services;

public class VenueService : IVenueService
{
    private readonly AppDbContext _context;

    public VenueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Venue>> GetVenuesAsync()
    {
        return await _context.Venues
            .AsNoTracking()
            .Include(venue => venue.EventVenues)
                .ThenInclude(eventVenue => eventVenue.Event)
            .OrderBy(venue => venue.VenueName)
            .ToListAsync();
    }

    public async Task<Venue?> GetVenueByIdAsync(int id)
    {
        return await _context.Venues
            .AsNoTracking()
            .Include(venue => venue.EventVenues)
                .ThenInclude(eventVenue => eventVenue.Event)
            .FirstOrDefaultAsync(venue => venue.Id == id);
    }

    public async Task AddVenueAsync(Venue venue)
    {
        await _context.Venues.AddAsync(venue);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVenueAsync(Venue venue)
    {
        _context.Venues.Update(venue);
        await _context.SaveChangesAsync();
    }

    public async Task SetVenueAvailabilityAsync(int id, bool isAvailable)
    {
        var venue = await _context.Venues.FindAsync(id);

        if (venue is null)
        {
            return;
        }

        venue.IsAvailable = isAvailable;
        await _context.SaveChangesAsync();
    }
}
