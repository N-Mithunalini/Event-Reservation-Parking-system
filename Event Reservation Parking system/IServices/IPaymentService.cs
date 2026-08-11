namespace EventParkingReservationSystem.IServices;
public interface IPaymentService
{
    Task<object> GetForBookingAsync(int bookingId);
    Task<object> PayAsync(int bookingId);
    Task<object> GetByCustomerAsync(int customerId);
    Task<object> GetReceiptAsync(int paymentId);
}
