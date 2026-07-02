using System.ComponentModel.DataAnnotations;

namespace CommunityEvent.Models;

public class Event : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string EventName { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; } = DateTime.Today;

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int MaxParticipants { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();
    public ICollection<EventVenue> EventVenues { get; set; } = new List<EventVenue>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EventDate.Date < DateTime.Today)
        {
            yield return new ValidationResult(
                "Event date cannot be in the past.",
                new[] { nameof(EventDate) });
        }

        if (StartTime >= EndTime)
        {
            yield return new ValidationResult(
                "Start time must be before end time.",
                new[] { nameof(StartTime), nameof(EndTime) });
        }
    }
}
