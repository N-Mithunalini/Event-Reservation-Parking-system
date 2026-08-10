using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;

using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventParkingReservationSystem.Repositories;

public class ParkingRepository : IParkingRepository
{
    private readonly ApplicationDbContext _db;
    public ParkingRepository(ApplicationDbContext db) => _db = db;
    public Task<List<ParkingSlot>> GetByEventAsync(int eventId) => _db.ParkingSlots.Where(x => x.EventId == eventId).OrderBy(x => x.SlotNumber).ToListAsync();
    public Task<ParkingSlot?> GetByIdAsync(int id) => _db.ParkingSlots.FindAsync(id).AsTask();
    public Task AddRangeAsync(IEnumerable<ParkingSlot> slots) { _db.ParkingSlots.AddRange(slots); return Task.CompletedTask; }
    public Task DeleteAsync(ParkingSlot slot) { _db.ParkingSlots.Remove(slot); return Task.CompletedTask; }
    public Task<bool> IsReservedAsync(int slotId) => _db.ParkingReservations.AnyAsync(x => x.ParkingSlotId == slotId && (x.Booking!.Status == "Pending" || x.Booking.Status == "Confirmed"));
    public Task SaveAsync() => _db.SaveChangesAsync();
}
