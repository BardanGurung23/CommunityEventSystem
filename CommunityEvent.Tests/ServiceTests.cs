using CommunityEvent.Data;
using CommunityEvent.Exceptions;
using CommunityEvent.Models;
using CommunityEvent.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Tests;

public class ServiceTests
{
    [Fact]
    public async Task EventService_AddEventAsync_SavesValidEvent()
    {
        await using var context = CreateContext();
        var service = new EventService(context);
        var eventItem = CreateEvent("Community Workshop");

        await service.AddEventAsync(eventItem);

        var savedEvent = await context.Events.SingleAsync();
        Assert.Equal("Community Workshop", savedEvent.EventName);
        Assert.True(savedEvent.IsActive);
    }

    [Fact]
    public async Task EventService_AddEventAsync_RejectsPastEventDate()
    {
        await using var context = CreateContext();
        var service = new EventService(context);
        var eventItem = CreateEvent("Past Event");
        eventItem.EventDate = DateTime.Today.AddDays(-1);

        await Assert.ThrowsAsync<InvalidEventDateException>(() => service.AddEventAsync(eventItem));
    }

    [Fact]
    public async Task EventService_AddEventAsync_RejectsInvalidTimeRange()
    {
        await using var context = CreateContext();
        var service = new EventService(context);
        var eventItem = CreateEvent("Invalid Time Event");
        eventItem.StartTime = TimeSpan.FromHours(12);
        eventItem.EndTime = TimeSpan.FromHours(10);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(eventItem));
        Assert.Contains("Start time", exception.Message);
    }

    [Fact]
    public async Task EventService_GetUpcomingEventsAsync_ReturnsOnlyActiveFutureEvents()
    {
        await using var context = CreateContext();
        context.Events.AddRange(
            CreateEvent("Upcoming Active"),
            CreateEvent("Inactive Future", isActive: false),
            CreateEvent("Past Active", daysFromToday: -2));
        await context.SaveChangesAsync();
        var service = new EventService(context);

        var result = await service.GetUpcomingEventsAsync();

        Assert.Single(result);
        Assert.Equal("Upcoming Active", result[0].EventName);
    }

    [Fact]
    public async Task RegistrationService_RegisterParticipantAsync_CreatesPendingRegistration()
    {
        await using var context = CreateContext();
        var participant = CreateParticipant("asha@example.com");
        var eventItem = CreateEvent("Food Drive");
        context.Participants.Add(participant);
        context.Events.Add(eventItem);
        await context.SaveChangesAsync();
        var service = new RegistrationService(context);

        var registration = await service.RegisterParticipantAsync(participant.Id, eventItem.Id, "Happy to help");

        Assert.Equal(RegistrationStatus.Pending, registration.Status);
        Assert.Equal("Happy to help", registration.Notes);
        Assert.Equal(1, await context.Registrations.CountAsync());
    }

    [Fact]
    public async Task RegistrationService_RegisterParticipantAsync_PreventsDuplicateActiveRegistration()
    {
        await using var context = CreateContext();
        var participant = CreateParticipant("sam@example.com");
        var eventItem = CreateEvent("Skills Fair");
        context.Participants.Add(participant);
        context.Events.Add(eventItem);
        await context.SaveChangesAsync();
        var service = new RegistrationService(context);

        await service.RegisterParticipantAsync(participant.Id, eventItem.Id);

        await Assert.ThrowsAsync<DuplicateRegistrationException>(() => service.RegisterParticipantAsync(participant.Id, eventItem.Id));
    }

    [Fact]
    public async Task RegistrationService_ConfirmRegistrationAsync_ChangesStatusToConfirmed()
    {
        await using var context = CreateContext();
        var registration = await SeedRegistrationAsync(context);
        var service = new RegistrationService(context);

        await service.ConfirmRegistrationAsync(registration.Id);

        var updated = await context.Registrations.FindAsync(registration.Id);
        Assert.Equal(RegistrationStatus.Confirmed, updated!.Status);
    }

    [Fact]
    public async Task RegistrationService_CancelRegistrationAsync_ChangesStatusToCancelled()
    {
        await using var context = CreateContext();
        var registration = await SeedRegistrationAsync(context);
        var service = new RegistrationService(context);

        await service.CancelRegistrationAsync(registration.Id);

        var updated = await context.Registrations.FindAsync(registration.Id);
        Assert.Equal(RegistrationStatus.Cancelled, updated!.Status);
    }

    [Fact]
    public async Task ReportService_GetEventRegistrationCountsAsync_IgnoresCancelledRegistrations()
    {
        await using var context = CreateContext();
        var eventItem = CreateEvent("Community Food Support Day");
        context.Events.Add(eventItem);
        await context.SaveChangesAsync();
        context.Registrations.AddRange(
            new Registration { EventId = eventItem.Id, ParticipantId = 1, Status = RegistrationStatus.Pending },
            new Registration { EventId = eventItem.Id, ParticipantId = 2, Status = RegistrationStatus.Confirmed },
            new Registration { EventId = eventItem.Id, ParticipantId = 3, Status = RegistrationStatus.Cancelled });
        await context.SaveChangesAsync();
        var service = new ReportService(context);

        var result = await service.GetEventRegistrationCountsAsync();

        Assert.Equal(2, result["Community Food Support Day"]);
    }

    [Fact]
    public async Task VenueService_SetVenueAvailabilityAsync_UpdatesAvailability()
    {
        await using var context = CreateContext();
        var venue = new Venue { VenueName = "Main Hall", Address = "Town Centre", Capacity = 50, IsAvailable = true };
        context.Venues.Add(venue);
        await context.SaveChangesAsync();
        var service = new VenueService(context);

        await service.SetVenueAvailabilityAsync(venue.Id, false);

        var updated = await context.Venues.FindAsync(venue.Id);
        Assert.False(updated!.IsAvailable);
    }

    [Fact]
    public async Task ActivityService_DeactivateActivityAsync_MarksActivityInactive()
    {
        await using var context = CreateContext();
        var activity = new Activity { ActivityName = "Workshop", ActivityType = "Education", Description = "Learning session", IsActive = true };
        context.Activities.Add(activity);
        await context.SaveChangesAsync();
        var service = new ActivityService(context);

        await service.DeactivateActivityAsync(activity.Id);

        var updated = await context.Activities.FindAsync(activity.Id);
        Assert.False(updated!.IsActive);
    }

    [Fact]
    public void PasswordHasher_VerifiesParticipantPasswordHash()
    {
        var participant = CreateParticipant("newuser@example.com");
        var hasher = new PasswordHasher<Participant>();
        participant.PasswordHash = hasher.HashPassword(participant, "participant123");

        var result = hasher.VerifyHashedPassword(participant, participant.PasswordHash, "participant123");

        Assert.NotEqual(PasswordVerificationResult.Failed, result);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Event CreateEvent(string name, int daysFromToday = 5, bool isActive = true)
    {
        return new Event
        {
            EventName = name,
            EventDate = DateTime.Today.AddDays(daysFromToday),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(11),
            Description = $"{name} description",
            MaxParticipants = 20,
            IsActive = isActive
        };
    }

    private static Participant CreateParticipant(string email)
    {
        return new Participant("Test Participant", email, "9800000000")
        {
            IsActive = true
        };
    }

    private static async Task<Registration> SeedRegistrationAsync(AppDbContext context)
    {
        var participant = CreateParticipant("seed@example.com");
        var eventItem = CreateEvent("Seed Event");
        context.Participants.Add(participant);
        context.Events.Add(eventItem);
        await context.SaveChangesAsync();

        var registration = new Registration
        {
            ParticipantId = participant.Id,
            EventId = eventItem.Id,
            Status = RegistrationStatus.Pending,
            RegistrationDate = DateTime.UtcNow
        };

        context.Registrations.Add(registration);
        await context.SaveChangesAsync();
        return registration;
    }
}
