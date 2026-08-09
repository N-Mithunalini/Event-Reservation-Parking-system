using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;
public interface INotificationRepository
{
    Task<List<Notification>> GetByCustomerAsync(int customerId);
    Task<Notification?> GetByIdAsync(int id);
    Task AddAsync(Notification notification);
    Task SaveAsync();
}
