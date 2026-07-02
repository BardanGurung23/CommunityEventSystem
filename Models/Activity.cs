using System.ComponentModel.DataAnnotations;

namespace CommunityEvent.Models;

public class Activity
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string ActivityName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ActivityType { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();
}
