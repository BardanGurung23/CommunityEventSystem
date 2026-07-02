namespace CommunityEvent.Exceptions;

public class InvalidEventDateException : Exception
{
    public InvalidEventDateException()
        : base("Event date cannot be in the past.")
    {
    }
}
