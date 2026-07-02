using CommunityEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEvent.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<EventActivity> EventActivities => Set<EventActivity>();
    public DbSet<EventVenue> EventVenues => Set<EventVenue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventActivity>()
            .HasKey(eventActivity => new { eventActivity.EventId, eventActivity.ActivityId });

        modelBuilder.Entity<EventActivity>()
            .HasOne(eventActivity => eventActivity.Event)
            .WithMany(eventItem => eventItem.EventActivities)
            .HasForeignKey(eventActivity => eventActivity.EventId);

        modelBuilder.Entity<EventActivity>()
            .HasOne(eventActivity => eventActivity.Activity)
            .WithMany(activity => activity.EventActivities)
            .HasForeignKey(eventActivity => eventActivity.ActivityId);

        modelBuilder.Entity<EventVenue>()
            .HasKey(eventVenue => new { eventVenue.EventId, eventVenue.VenueId });

        modelBuilder.Entity<EventVenue>()
            .HasOne(eventVenue => eventVenue.Event)
            .WithMany(eventItem => eventItem.EventVenues)
            .HasForeignKey(eventVenue => eventVenue.EventId);

        modelBuilder.Entity<EventVenue>()
            .HasOne(eventVenue => eventVenue.Venue)
            .WithMany(venue => venue.EventVenues)
            .HasForeignKey(eventVenue => eventVenue.VenueId);

        modelBuilder.Entity<Registration>()
            .HasIndex(registration => new { registration.ParticipantId, registration.EventId })
            .IsUnique();

        modelBuilder.Entity<Registration>()
            .HasOne(registration => registration.Participant)
            .WithMany(participant => participant.Registrations)
            .HasForeignKey(registration => registration.ParticipantId);

        modelBuilder.Entity<Registration>()
            .HasOne(registration => registration.Event)
            .WithMany(eventItem => eventItem.Registrations)
            .HasForeignKey(registration => registration.EventId);

        modelBuilder.Entity<Registration>()
            .Property(registration => registration.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
