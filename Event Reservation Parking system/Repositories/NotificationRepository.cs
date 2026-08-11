using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _db;
    public NotificationRepository(ApplicationDbContext db) => _db = db;
    public Task<List<Notification>> GetByCustomerAsync(int customerId) => _db.Notifications.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt).ToListAsync();
    public Task<Notification?> GetByIdAsync(int id) => _db.Notifications.FindAsync(id).AsTask();
    public Task AddAsync(Notification notification) { _db.Notifications.Add(notification); return Task.CompletedTask; }
    public Task SaveAsync() => _db.SaveChangesAsync();
}
