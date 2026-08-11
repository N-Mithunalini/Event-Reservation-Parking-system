using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _db;

    public SeatRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<Seat>> GetByEventAsync(int eventId)
    {
        return _db.Seats
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.Row)
            .ThenBy(x => x.Column)
            .ToListAsync();
    }

    public Task<Seat?> GetByIdAsync(int id)
    {
        return _db.Seats
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task AddRangeAsync(IEnumerable<Seat> seats)
    {
        _db.Seats.AddRange(seats);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Seat seat)
    {
        _db.Seats.Remove(seat);
        return Task.CompletedTask;
    }

    public Task<bool> IsBookedAsync(int seatId)
    {
        return _db.BookingSeats
            .AnyAsync(x =>
                x.SeatId == seatId &&
                x.Booking != null &&
                (
                    x.Booking.Status == "Pending" ||
                    x.Booking.Status == "Confirmed"
                )
            );
    }

    public Task SaveAsync()
    {
        return _db.SaveChangesAsync();
    }
}