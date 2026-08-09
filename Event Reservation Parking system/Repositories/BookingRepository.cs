using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _db;
    public BookingRepository(ApplicationDbContext db) => _db = db;

    public Task<Booking?> GetByIdAsync(int id) => _db.Bookings
        .Include(x => x.Customer).Include(x => x.Event)
        .Include(x => x.BookingSeats).ThenInclude(x => x.Seat)
        .Include(x => x.ParkingReservation).ThenInclude(x => x!.ParkingSlot)
        .Include(x => x.Payment)
        .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Booking>> GetByCustomerAsync(int customerId) => _db.Bookings
        .Include(x => x.Event).Include(x => x.BookingSeats).ThenInclude(x => x.Seat)
        .Include(x => x.ParkingReservation).ThenInclude(x => x!.ParkingSlot)
        .Include(x => x.Payment)
        .Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public Task<List<Booking>> GetByEventAsync(int eventId) => _db.Bookings
        .Include(x => x.Customer).Where(x => x.EventId == eventId).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public Task AddAsync(Booking booking) { _db.Bookings.Add(booking); return Task.CompletedTask; }
    public Task SaveAsync() => _db.SaveChangesAsync();
}
