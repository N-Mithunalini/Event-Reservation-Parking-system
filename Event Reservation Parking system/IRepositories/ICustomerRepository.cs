using EventParkingReservationSystem.Models;

namespace EventParkingReservationSystem.IRepositories; public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(string? search = null);

    Task<Customer?> GetByIdAsync(int id);

    Task<Customer?> GetByEmailAsync(string email);

    Task<Customer?> GetByVerificationTokenAsync(string token);

    Task<Customer?> GetByResetTokenAsync(string token);

    Task<bool> EmailExistsAsync(string email);

    Task<bool> HasActiveBookingsAsync(int customerId);

    Task AddAsync(Customer customer);

    Task SaveAsync();
}