using EventParkingReservationSystem.Data.EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;

namespace EventParkingReservationSystem.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _db;
    public SeatRepository(ApplicationDbContext db) => _db = db;
    public Task<List<Seat>> GetByEventAsync(int eventId) => _db.Seats.Where(x => x.EventId == eventId).OrderBy(x => x.Row).ThenBy(x => x.Column).ToListAsync();
    public Task<Seat?> GetByIdAsync(int id) => _db.Seats.FindAsync(id).AsTask();
    public Task AddRangeAsync(IEnumerable<Seat> seats) { _db.Seats.AddRange(seats); return Task.CompletedTask; }
    public Task DeleteAsync(Seat seat) { _db.Seats.Remove(seat); return Task.CompletedTask; }
    public Task<bool> IsBookedAsync(int seatId) => _db.BookingSeats.AnyAsync(x => x.SeatId == seatId && (x.Booking!.Status == "Pending" || x.Booking.Status == "Confirmed"));
    public Task SaveAsync() => _db.SaveChangesAsync();
}
