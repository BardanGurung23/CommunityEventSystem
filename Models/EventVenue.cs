namespace CommunityEvent.Models;

public class EventVenue
{
    public int EventId { get; set; }
    public int VenueId { get; set; }

    public Event? Event { get; set; }
    public Venue? Venue { get; set; }
}
