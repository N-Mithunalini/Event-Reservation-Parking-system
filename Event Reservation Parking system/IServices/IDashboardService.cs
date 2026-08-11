namespace EventParkingReservationSystem.IServices;
public interface IDashboardService
{
    Task<object> AdminAsync();
    Task<object> CustomerAsync(int customerId);
}
