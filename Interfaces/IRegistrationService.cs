using CommunityEvent.Models;

namespace CommunityEvent.Interfaces;

public interface IRegistrationService
{
    Task<List<Registration>> GetRegistrationsAsync();
    Task<List<Registration>> GetRegistrationsForParticipantAsync(int participantId);
    Task<Registration> RegisterParticipantAsync(int participantId, int eventId, string notes = "");
    Task ConfirmRegistrationAsync(int registrationId);
    Task CancelRegistrationAsync(int registrationId);
}
