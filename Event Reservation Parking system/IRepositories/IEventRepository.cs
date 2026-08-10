using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync(string? search = null, int? venueId = null, int? categoryId = null, DateTime? date = null);
    Task<Event?> GetByIdAsync(int id);
    Task AddAsync(Event ev);
    Task UpdateAsync(Event ev);
    Task DeleteAsync(Event ev);
    Task<int> GetBookedSeatCountAsync(int eventId);
    Task<bool> HasActiveBookingsAsync(int eventId);
    Task SaveAsync();
}
