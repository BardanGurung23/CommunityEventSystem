namespace CommunityEvent.Exceptions;

public class DuplicateRegistrationException : Exception
{
    public DuplicateRegistrationException()
        : base("This participant is already registered for the selected event.")
    {
    }
}
