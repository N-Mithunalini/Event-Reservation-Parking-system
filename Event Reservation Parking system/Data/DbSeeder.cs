using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Venues.AnyAsync()) return;

        var venue = new Venue { Name = "Colombo Convention Hall", Address = "Colombo", Capacity = 20 };
        var category = new EventCategory { Name = "Concert" };
        db.Venues.Add(venue);
        db.EventCategories.Add(category);
        await db.SaveChangesAsync();

        var ev = new Event
        {
            Name = "Demo Music Night",
            VenueId = venue.Id,
            CategoryId = category.Id,
            EventDate = DateTime.Today.AddDays(7),
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(21, 0, 0),
            TicketPrice = 1500,
            ParkingFee = 300,
            Capacity = 20
        };
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        for (int r = 0; r < 4; r++)
        {
            var row = ((char)('A' + r)).ToString();
            for (int c = 1; c <= 5; c++)
                db.Seats.Add(new Seat { EventId = ev.Id, Row = row, Column = c, SeatNumber = $"{row}{c}", Status = "Available" });
        }
        for (int i = 1; i <= 8; i++)
            db.ParkingSlots.Add(new ParkingSlot { EventId = ev.Id, SlotNumber = $"P{i:00}", Zone = "A", Fee = 300, Status = "Available" });

        await db.SaveChangesAsync();
    }
}
