using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;
public interface IPaymentRepository
{
    Task<Payment?> GetByBookingAsync(int bookingId);
    Task<List<Payment>> GetByCustomerAsync(int customerId);
    Task<Payment?> GetByIdAsync(int id);
    Task AddAsync(Payment payment);
    Task SaveAsync();
}
