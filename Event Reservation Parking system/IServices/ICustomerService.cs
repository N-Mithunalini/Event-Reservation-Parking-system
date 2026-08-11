using EventParkingReservationSystem.DTOs;

namespace EventParkingReservationSystem.IServices;

public interface ICustomerService
{
    Task<object> GetAllAsync(string? search);

    Task<object> GetByIdAsync(int id);

    Task<object> UpdateAsync(
        int id,
        CustomerUpdateDto dto);

    Task<object> DeactivateAsync(int id);

    Task<object> ReactivateAsync(int id);
}