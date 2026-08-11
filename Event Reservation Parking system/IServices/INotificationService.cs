namespace EventParkingReservationSystem.IServices;
public interface INotificationService
{
<<<<<<< HEAD
    Task CreateAsync(object customerId, string v);
=======
    Task CreateAsync(int customerId, string message);
    Task<object> GetByCustomerAsync(int customerId);
    Task<object> MarkReadAsync(int id);
>>>>>>> origin/master
}
