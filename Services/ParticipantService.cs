using CommunityEvent.Data;
using CommunityEvent.Exceptions;
using CommunityEvent.Interfaces;
using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Services;

public class ParticipantService : IParticipantService
{
    private readonly AppDbContext _context;

    public ParticipantService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Participant>> GetParticipantsAsync()
    {
        return await _context.Participants
            .AsNoTracking()
            .Include(participant => participant.Registrations)
                .ThenInclude(registration => registration.Event)
            .OrderBy(participant => participant.FullName)
            .ToListAsync();
    }

    public async Task<Participant?> GetParticipantByIdAsync(int id)
    {
        return await _context.Participants
            .AsNoTracking()
            .Include(participant => participant.Registrations)
                .ThenInclude(registration => registration.Event)
            .FirstOrDefaultAsync(participant => participant.Id == id);
    }

    public async Task AddParticipantAsync(Participant participant)
    {
        await _context.Participants.AddAsync(participant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateParticipantAsync(Participant participant)
    {
        _context.Participants.Update(participant);
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateParticipantAsync(int id)
    {
        var participant = await _context.Participants.FindAsync(id);

        if (participant is null)
        {
            throw new ParticipantNotFoundException(id);
        }

        participant.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
