using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<List<Booking>> GetByCustomerAsync(int customerId);
    Task<List<Booking>> GetByEventAsync(int eventId);
    Task AddAsync(Booking booking);
    Task SaveAsync();
}
