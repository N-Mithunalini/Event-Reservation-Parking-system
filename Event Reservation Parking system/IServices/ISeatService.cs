using EventParkingReservationSystem.DTOs;
using System.Threading.Tasks;
namespace EventParkingReservationSystem.IServices;
public interface ISeatService
{
    Task<object> GetByEventAsync(int eventId);
    Task<object> GenerateAsync(int eventId, SeatMapCreateDto dto);
    Task<object> UpdateAsync(int eventId, int seatId, SeatUpdateDto dto);
    Task<object> DeleteAsync(int eventId, int seatId);
}

