using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<ParkingSlot> ParkingSlots => Set<ParkingSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<ParkingReservation> ParkingReservations => Set<ParkingReservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(x => x.BookingNumber)
            .IsUnique();

        modelBuilder.Entity<BookingSeat>()
            .HasIndex(x => new { x.BookingId, x.SeatId })
            .IsUnique();

        modelBuilder.Entity<ParkingReservation>()
            .HasIndex(x => x.BookingId)
            .IsUnique();

        modelBuilder.Entity<Payment>()
            .HasIndex(x => x.BookingId)
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasIndex(x => new { x.EventId, x.SeatNumber })
            .IsUnique();

        modelBuilder.Entity<ParkingSlot>()
            .HasIndex(x => new { x.EventId, x.SlotNumber })
            .IsUnique();


        // Prevent SQL Server multiple cascade paths

        modelBuilder.Entity<Booking>()
            .HasOne(x => x.Event)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BookingSeat>()
            .HasOne(x => x.Booking)
            .WithMany(x => x.BookingSeats)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookingSeat>()
            .HasOne(x => x.Event)
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BookingSeat>()
            .HasOne(x => x.Seat)
            .WithMany(x => x.BookingSeats)
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ParkingReservation>()
            .HasOne(x => x.Booking)
            .WithOne(x => x.ParkingReservation)
            .HasForeignKey<ParkingReservation>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParkingReservation>()
            .HasOne(x => x.Event)
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ParkingReservation>()
            .HasOne(x => x.ParkingSlot)
            .WithMany(x => x.ParkingReservations)
            .HasForeignKey(x => x.ParkingSlotId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Event>()
            .Property(x => x.TicketPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Event>()
            .Property(x => x.ParkingFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ParkingSlot>()
            .Property(x => x.Fee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ParkingReservation>()
            .Property(x => x.ReservedFee)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}