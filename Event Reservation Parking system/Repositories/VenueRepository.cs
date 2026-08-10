using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly ApplicationDbContext _db;
    public VenueRepository(ApplicationDbContext db) => _db = db;
    public Task<List<Venue>> GetAllAsync() => _db.Venues.OrderBy(x => x.Name).ToListAsync();
    public Task<Venue?> GetByIdAsync(int id) => _db.Venues.FirstOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(Venue venue) { _db.Venues.Add(venue); return Task.CompletedTask; }
    public Task UpdateAsync(Venue venue) { _db.Venues.Update(venue); return Task.CompletedTask; }
    public Task DeleteAsync(Venue venue) { _db.Venues.Remove(venue); return Task.CompletedTask; }
    public Task<bool> HasUpcomingEventsAsync(int venueId) => _db.Events.AnyAsync(e => e.VenueId == venueId && e.EventDate >= DateTime.Today);
    public Task<bool> IsAvailableAsync(int venueId, DateTime date, TimeSpan start, TimeSpan end, int? excludeEventId = null) =>
        _db.Events.AllAsync(e => e.VenueId != venueId || e.EventDate.Date != date.Date ||
            (excludeEventId.HasValue && e.Id == excludeEventId.Value) ||
            end <= e.StartTime || start >= e.EndTime);
    public Task SaveAsync() => _db.SaveChangesAsync();
}
