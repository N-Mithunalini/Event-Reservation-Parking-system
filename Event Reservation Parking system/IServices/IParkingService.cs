using EventParkingReservationSystem.DTOs;
<<<<<<< HEAD
=======
namespace EventParkingReservationSystem.IServices;
>>>>>>> origin/master

namespace EventParkingReservationSystem.IServices;
public interface IParkingService
{
    Task<object> GetByEventAsync(int eventId);
    Task<object> GenerateAsync(int eventId, ParkingLayoutCreateDto dto);
    Task<object> UpdateAsync(int eventId, int slotId, ParkingSlotUpdateDto dto);
    Task<object> DeleteAsync(int eventId, int slotId);
}
