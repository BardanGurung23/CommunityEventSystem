using CommunityEvent.Models;

namespace CommunityEvent.Helpers;

public static class UserFactory
{
    public static User CreateUser(string userType)
    {
        return userType.Trim().ToLowerInvariant() switch
        {
            "admin" => new Admin(),
            "participant" => new Participant(),
            _ => throw new ArgumentException("Unknown user type.", nameof(userType))
        };
    }
}
