using System.ComponentModel.DataAnnotations;

namespace CommunityEvent.Models;

public class Participant : User
{
    public Participant()
    {
    }

    public Participant(string fullName, string email, string phoneNumber)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    [StringLength(80)]
    public string? EmergencyContact { get; set; }
}
