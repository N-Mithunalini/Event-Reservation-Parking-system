using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;
<<<<<<< Updated upstream
=======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
>>>>>>> Stashed changes

namespace EventParkingReservationSystem.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _db;
    public EventRepository(ApplicationDbContext db) => _db = db;

    public async Task<List<Event>> GetAllAsync(string? search = null, int? venueId = null, int? categoryId = null, DateTime? date = null)
    {
        var q = _db.Events.Include(x => x.Venue).Include(x => x.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Name.Contains(search));
        if (venueId.HasValue) q = q.Where(x => x.VenueId == venueId);
        if (categoryId.HasValue) q = q.Where(x => x.CategoryId == categoryId);
        if (date.HasValue) q = q.Where(x => x.EventDate.Date == date.Value.Date);
        return await q.OrderBy(x => x.EventDate).ThenBy(x => x.StartTime).ToListAsync();
    }

    public Task<Event?> GetByIdAsync(int id) => _db.Events
        .Include(x => x.Venue).Include(x => x.Category)
        .FirstOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(Event ev) { _db.Events.Add(ev); return Task.CompletedTask; }
    public Task UpdateAsync(Event ev) { _db.Events.Update(ev); return Task.CompletedTask; }
    public Task DeleteAsync(Event ev) { _db.Events.Remove(ev); return Task.CompletedTask; }
    public Task<int> GetBookedSeatCountAsync(int eventId) =>
        _db.BookingSeats.CountAsync(bs => bs.Seat!.EventId == eventId &&
            (bs.Booking!.Status == "Pending" || bs.Booking.Status == "Confirmed"));
    public Task<bool> HasActiveBookingsAsync(int eventId) =>
        _db.Bookings.AnyAsync(b => b.EventId == eventId && (b.Status == "Pending" || b.Status == "Confirmed"));
    public Task SaveAsync() => _db.SaveChangesAsync();
}
