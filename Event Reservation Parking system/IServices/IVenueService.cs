using EventParkingReservationSystem.DTOs;
namespace EventParkingReservationSystem.IServices;

public interface IVenueService
{
    Task<object> GetAllAsync();
    Task<object> GetByIdAsync(int id);
    Task<object> CreateAsync(VenueDto dto);
    Task<object> UpdateAsync(int id, VenueDto dto);
    Task<object> DeleteAsync(int id);
    Task<object> AvailableAsync(int venueId, DateTime date, TimeSpan start, TimeSpan end);
}
