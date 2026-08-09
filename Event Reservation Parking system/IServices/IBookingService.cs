using EventParkingReservationSystem.DTOs;
namespace EventParkingReservationSystem.IServices;
public interface IBookingService
{
    Task<object> CreateAsync(BookingCreateDto dto);
    Task<object> GetByIdAsync(int id);
    Task<object> GetByCustomerAsync(int customerId);
    Task<object> GetByEventAsync(int eventId);
    Task<object> CancelAsync(int id);
    Task<object> HoldStatusAsync(int id);
}
