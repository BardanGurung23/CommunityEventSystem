using CommunityEvent.Data;
using CommunityEvent.Exceptions;
using CommunityEvent.Interfaces;
using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Services;

public class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _context;

    public RegistrationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Registration>> GetRegistrationsAsync()
    {
        return await BaseRegistrationQuery()
            .OrderByDescending(registration => registration.RegistrationDate)
            .ToListAsync();
    }

    public async Task<List<Registration>> GetRegistrationsForParticipantAsync(int participantId)
    {
        return await BaseRegistrationQuery()
            .Where(registration => registration.ParticipantId == participantId)
            .OrderBy(registration => registration.Event!.EventDate)
            .ToListAsync();
    }

    public async Task<Registration> RegisterParticipantAsync(int participantId, int eventId, string notes = "")
    {
        var participant = await _context.Participants.FindAsync(participantId);

        if (participant is null || !participant.IsActive)
        {
            throw new ParticipantNotFoundException(participantId);
        }

        var eventItem = await _context.Events
            .Include(item => item.Registrations)
            .Include(item => item.EventVenues)
                .ThenInclude(eventVenue => eventVenue.Venue)
            .FirstOrDefaultAsync(item => item.Id == eventId);

        if (eventItem is null)
        {
            throw new EventNotFoundException(eventId);
        }

        if (!eventItem.IsActive || eventItem.EventDate.Date < DateTime.Today)
        {
            throw new RegistrationClosedException();
        }

        var alreadyRegistered = await _context.Registrations
            .AnyAsync(registration => registration.ParticipantId == participantId &&
                                      registration.EventId == eventId &&
                                      registration.Status != RegistrationStatus.Cancelled);

        if (alreadyRegistered)
        {
            throw new DuplicateRegistrationException();
        }

        var activeRegistrationCount = eventItem.Registrations
            .Count(registration => registration.Status != RegistrationStatus.Cancelled);
        var venueCapacity = eventItem.EventVenues
            .Where(eventVenue => eventVenue.Venue is not null)
            .Select(eventVenue => eventVenue.Venue!.Capacity)
            .DefaultIfEmpty(eventItem.MaxParticipants)
            .Min();
        var allowedCapacity = Math.Min(eventItem.MaxParticipants, venueCapacity);

        if (activeRegistrationCount >= allowedCapacity)
        {
            throw new VenueCapacityExceededException();
        }

        var registration = new Registration
        {
            ParticipantId = participantId,
            EventId = eventId,
            Notes = notes,
            Status = RegistrationStatus.Pending,
            RegistrationDate = DateTime.UtcNow
        };

        await _context.Registrations.AddAsync(registration);
        await _context.SaveChangesAsync();

        return registration;
    }

    public async Task ConfirmRegistrationAsync(int registrationId)
    {
        var registration = await _context.Registrations.FindAsync(registrationId);

        if (registration is null)
        {
            return;
        }

        registration.Status = RegistrationStatus.Confirmed;
        await _context.SaveChangesAsync();
    }

    public async Task CancelRegistrationAsync(int registrationId)
    {
        var registration = await _context.Registrations.FindAsync(registrationId);

        if (registration is null)
        {
            return;
        }

        registration.Status = RegistrationStatus.Cancelled;
        await _context.SaveChangesAsync();
    }

    private IQueryable<Registration> BaseRegistrationQuery()
    {
        return _context.Registrations
            .AsNoTracking()
            .Include(registration => registration.Participant)
            .Include(registration => registration.Event);
    }
}
