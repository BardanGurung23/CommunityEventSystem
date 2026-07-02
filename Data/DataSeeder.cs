using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CommunityEvent.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        var passwordHasher = new PasswordHasher<Participant>();
        var participantsWithoutPasswords = await context.Participants
            .Where(participant => participant.PasswordHash == null)
            .ToListAsync();

        foreach (var participant in participantsWithoutPasswords)
        {
            participant.PasswordHash = passwordHasher.HashPassword(participant, "participant123");
        }

        if (participantsWithoutPasswords.Count > 0)
        {
            await context.SaveChangesAsync();
        }

        if (await context.Events.AnyAsync())
        {
            return;
        }

        var venues = new List<Venue>
        {
            new()
            {
                VenueName = "Riverside Community Hall",
                Address = "12 Riverside Road",
                Capacity = 80,
                Description = "Large indoor hall with projector, tables, and accessible entrance.",
                IsAvailable = true
            },
            new()
            {
                VenueName = "Green Park Pavilion",
                Address = "Green Park",
                Capacity = 120,
                Description = "Outdoor covered venue suitable for fairs and family activities.",
                IsAvailable = true
            }
        };

        var activities = new List<Activity>
        {
            new()
            {
                ActivityName = "Food Drive",
                ActivityType = "Volunteering",
                Description = "Collect and sort donated food for local families.",
                IsActive = true
            },
            new()
            {
                ActivityName = "Live Workshop",
                ActivityType = "Education",
                Description = "Skill-sharing workshop led by local volunteers.",
                IsActive = true
            }
        };

        var participants = new List<Participant>
        {
            new("Asha Gurung", "asha@example.com", "9800000001"),
            new("Sam Taylor", "sam@example.com", "9800000002")
        };

        foreach (var participant in participants)
        {
            participant.PasswordHash = passwordHasher.HashPassword(participant, "participant123");
        }

        var events = new List<Event>
        {
            new()
            {
                EventName = "Community Food Support Day",
                EventDate = DateTime.Today.AddDays(7),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(14, 0, 0),
                Description = "A local event for collecting, sorting, and distributing food donations.",
                MaxParticipants = 60,
                IsActive = true,
                EventVenues = new List<EventVenue>
                {
                    new() { Venue = venues[0] }
                },
                EventActivities = new List<EventActivity>
                {
                    new() { Activity = activities[0] }
                }
            },
            new()
            {
                EventName = "Neighbourhood Skills Fair",
                EventDate = DateTime.Today.AddDays(14),
                StartTime = new TimeSpan(11, 0, 0),
                EndTime = new TimeSpan(16, 0, 0),
                Description = "A community fair with practical workshops, advice stalls, and volunteer activities.",
                MaxParticipants = 90,
                IsActive = true,
                EventVenues = new List<EventVenue>
                {
                    new() { Venue = venues[1] }
                },
                EventActivities = new List<EventActivity>
                {
                    new() { Activity = activities[1] }
                }
            }
        };

        await context.Participants.AddRangeAsync(participants);
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}
