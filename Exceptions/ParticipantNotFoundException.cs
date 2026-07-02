namespace CommunityEvent.Exceptions;

public class ParticipantNotFoundException : Exception
{
    public ParticipantNotFoundException(int participantId)
        : base($"Participant with ID {participantId} was not found.")
    {
    }
}
