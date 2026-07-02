using System.ComponentModel.DataAnnotations;

namespace CommunityEvent.Models;

public class Venue
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string VenueName { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int Capacity { get; set; }

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public ICollection<EventVenue> EventVenues { get; set; } = new List<EventVenue>();
}
