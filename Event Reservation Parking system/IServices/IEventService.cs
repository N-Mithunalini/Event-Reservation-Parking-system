using EventParkingReservationSystem.DTOs;
namespace EventParkingReservationSystem.IServices;

public interface IEventService
{
    Task<object> GetAllAsync(string? search, int? venueId, int? categoryId, DateTime? date);
    Task<object> GetByIdAsync(int id);
    Task<object> CreateAsync(EventDto dto);
    Task<object> UpdateAsync(int id, EventDto dto);
    Task<object> DeleteAsync(int id);
}
