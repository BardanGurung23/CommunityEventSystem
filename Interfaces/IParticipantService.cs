using CommunityEvent.Models;

namespace CommunityEvent.Interfaces;

public interface IParticipantService
{
    Task<List<Participant>> GetParticipantsAsync();
    Task<Participant?> GetParticipantByIdAsync(int id);
    Task AddParticipantAsync(Participant participant);
    Task UpdateParticipantAsync(Participant participant);
    Task DeactivateParticipantAsync(int id);
}
