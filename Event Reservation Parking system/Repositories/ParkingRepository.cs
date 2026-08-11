using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class ParkingRepository : IParkingRepository
{
    private readonly ApplicationDbContext _db;

    public ParkingRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<ParkingSlot>> GetByEventAsync(int eventId)
    {
        return _db.ParkingSlots
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.SlotNumber)
            .ToListAsync();
    }

    public Task<ParkingSlot?> GetByIdAsync(int id)
    {
        return _db.ParkingSlots
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task AddRangeAsync(IEnumerable<ParkingSlot> slots)
    {
        _db.ParkingSlots.AddRange(slots);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ParkingSlot slot)
    {
        _db.ParkingSlots.Remove(slot);
        return Task.CompletedTask;
    }

    public Task<bool> IsReservedAsync(int slotId)
    {
        return _db.ParkingReservations
            .AnyAsync(x =>
                x.ParkingSlotId == slotId &&
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