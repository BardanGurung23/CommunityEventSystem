using System.ComponentModel.DataAnnotations;

namespace CommunityEvent.Models;

public class Registration
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public int EventId { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    public Participant? Participant { get; set; }
    public Event? Event { get; set; }
}
