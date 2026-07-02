namespace CommunityEvent.Exceptions;

public class RegistrationClosedException : Exception
{
    public RegistrationClosedException()
        : base("Registration is closed for this event.")
    {
    }
}
