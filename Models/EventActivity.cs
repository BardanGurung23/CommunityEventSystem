namespace CommunityEvent.Models;

public class EventActivity
{
    public int EventId { get; set; }
    public int ActivityId { get; set; }

    public Event? Event { get; set; }
    public Activity? Activity { get; set; }
}
