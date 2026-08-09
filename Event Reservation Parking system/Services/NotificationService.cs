using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;

namespace EventParkingReservationSystem.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    public NotificationService(INotificationRepository repo) => _repo = repo;

    public async Task CreateAsync(int customerId, string message)
    {
        await _repo.AddAsync(new Notification { CustomerId = customerId, Message = message });
        await _repo.SaveAsync();
    }

    public async Task<object> GetByCustomerAsync(int customerId) => await _repo.GetByCustomerAsync(customerId);

    public async Task<object> MarkReadAsync(int id)
    {
        var n = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Notification not found.");
        n.IsRead = true; await _repo.SaveAsync(); return n;
    }
}
