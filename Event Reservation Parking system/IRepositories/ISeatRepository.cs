using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;

public interface ISeatRepository
{
    Task<List<Seat>> GetByEventAsync(int eventId);
    Task<Seat?> GetByIdAsync(int id);
    Task AddRangeAsync(IEnumerable<Seat> seats);
    Task DeleteAsync(Seat seat);
    Task<bool> IsBookedAsync(int seatId);
    Task SaveAsync();
}
