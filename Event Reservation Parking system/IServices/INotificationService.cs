namespace EventParkingReservationSystem.IServices;

public interface INotificationService
{
    Task CreateAsync(object customerId, string v);
}
