using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IServices;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;
    public DashboardService(ApplicationDbContext db) => _db = db;

    public async Task<object> AdminAsync() => new
    {
        TotalEvents = await _db.Events.CountAsync(),
        TotalBookings = await _db.Bookings.CountAsync(),
        AvailableSeats = await _db.Seats.CountAsync(x => x.Status == "Available"),
        OccupiedParkingSlots = await _db.ParkingSlots.CountAsync(x => x.Status != "Available"),
        TotalRevenue = await _db.Payments.Where(x => x.Status == "Completed").SumAsync(x => (decimal?)x.Amount) ?? 0,
        TotalCustomers = await _db.Customers.CountAsync()
    };

    public async Task<object> CustomerAsync(int customerId) => new
    {
        UpcomingBookings = await _db.Bookings.CountAsync(x => x.CustomerId == customerId && x.Event!.EventDate >= DateTime.Today),
        ReservedParking = await _db.ParkingReservations.CountAsync(x => x.Booking!.CustomerId == customerId),
        RecentPayments = await _db.Payments.CountAsync(x => x.Booking!.CustomerId == customerId),
        UnreadNotifications = await _db.Notifications.CountAsync(x => x.CustomerId == customerId && !x.IsRead)
    };
}
