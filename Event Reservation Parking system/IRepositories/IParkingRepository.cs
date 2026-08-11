using EventParkingReservationSystem.Models;
<<<<<<< HEAD
=======
namespace EventParkingReservationSystem.IRepositories;
>>>>>>> origin/master

namespace EventParkingReservationSystem.IRepositories;
public interface IParkingRepository
{
    Task<List<ParkingSlot>> GetByEventAsync(int eventId);
    Task<ParkingSlot?> GetByIdAsync(int id);
    Task AddRangeAsync(IEnumerable<ParkingSlot> slots);
    Task DeleteAsync(ParkingSlot slot);
    Task<bool> IsReservedAsync(int slotId);
    Task SaveAsync();
}
