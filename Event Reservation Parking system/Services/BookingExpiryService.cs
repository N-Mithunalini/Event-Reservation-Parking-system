using System.Data;
using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IServices;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Services;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingExpiryService> _logger;

    public BookingExpiryService(IServiceScopeFactory scopeFactory, ILogger<BookingExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var notifications = scope.ServiceProvider.GetRequiredService<INotificationService>();

                await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, stoppingToken);

                var expired = await db.Bookings
                    .Include(b => b.BookingSeats)
                        .ThenInclude(bs => bs.Seat)
                    .Include(b => b.ParkingReservation)
                        .ThenInclude(pr => pr!.ParkingSlot)
                    .Where(b => b.Status == "Pending" && b.HoldExpiresAt != null && b.HoldExpiresAt <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                var expiredInfo = new List<(int CustomerId, string BookingNumber)>();

                foreach (var booking in expired)
                {
                    booking.Status = "Expired";
                    booking.HoldExpiresAt = null;

                    foreach (var bookingSeat in booking.BookingSeats)
                        if (bookingSeat.Seat is not null)
                            bookingSeat.Seat.Status = "Available";

                    if (booking.ParkingReservation?.ParkingSlot is not null)
                        booking.ParkingReservation.ParkingSlot.Status = "Available";

                    expiredInfo.Add((booking.CustomerId, booking.BookingNumber));
                }
