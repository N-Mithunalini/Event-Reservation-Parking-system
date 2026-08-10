using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;

public interface ICategoryRepository
{
    Task<List<EventCategory>> GetAllAsync();
    Task<EventCategory?> GetByIdAsync(int id);
    Task AddAsync(EventCategory category);
    Task UpdateAsync(EventCategory category);
    Task DeleteAsync(EventCategory category);
    Task<bool> IsInUseAsync(int id);
    Task SaveAsync();
}
