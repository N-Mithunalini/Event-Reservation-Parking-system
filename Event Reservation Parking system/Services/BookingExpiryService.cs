using EventParkingReservationSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Services;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BookingExpiryService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();


            var expiredBookings =
                await db.Bookings
                    .Include(x => x.BookingSeats)
                        .ThenInclude(x => x.Seat)
                    .Include(x => x.ParkingReservation)
                        .ThenInclude(x => x.ParkingSlot)
                    .Where(x =>
                        x.Status == "Pending" &&
                        x.HoldExpiresAt.HasValue &&
                        x.HoldExpiresAt.Value <= DateTime.UtcNow
                    )
                    .ToListAsync(stoppingToken);


            foreach (var booking in expiredBookings)
            {
                booking.Status = "Expired";

                foreach (var bookingSeat in booking.BookingSeats)
                {
                    if (bookingSeat.Seat != null)
                    {
                        bookingSeat.Seat.Status = "Available";
                    }
                }

                if (booking.ParkingReservation?.ParkingSlot != null)
                {
                    booking.ParkingReservation.ParkingSlot.Status =
                        "Available";
                }
            }


            if (expiredBookings.Count > 0)
            {
                await db.SaveChangesAsync(stoppingToken);
            }


            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken
            );
        }
    }
}