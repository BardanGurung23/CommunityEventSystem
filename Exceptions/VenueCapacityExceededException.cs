namespace CommunityEvent.Exceptions;

public class VenueCapacityExceededException : Exception
{
    public VenueCapacityExceededException()
        : base("Registration cannot be completed because the event or venue capacity has been reached.")
    {
    }
}
