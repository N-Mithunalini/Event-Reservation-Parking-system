using EventParkingReservationSystem.DTOs;
namespace EventParkingReservationSystem.IServices;

public interface ICategoryService
{
    Task<object> GetAllAsync();
    Task<object> CreateAsync(CategoryDto dto);
    Task<object> UpdateAsync(int id, CategoryDto dto);
    Task<object> DeleteAsync(int id);
}
