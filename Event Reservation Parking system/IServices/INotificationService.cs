namespace EventParkingReservationSystem.IServices;
public interface INotificationService
{
    Task CreateAsync(int customerId, string message);
    Task<object> GetByCustomerAsync(int customerId);
    Task<object> MarkReadAsync(int id);
}
