using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _db;
    public PaymentRepository(ApplicationDbContext db) => _db = db;

    public Task<Payment?> GetByBookingAsync(int bookingId) =>
        _db.Payments.FirstOrDefaultAsync(x => x.BookingId == bookingId);

    public Task<List<Payment>> GetByCustomerAsync(int customerId) =>
        _db.Payments
            .Include(x => x.Booking)
                .ThenInclude(x => x!.Event)
            .Where(x => x.Booking!.CustomerId == customerId)
            .OrderByDescending(x => x.PaidAt)
            .ToListAsync();

    public Task<Payment?> GetByIdAsync(int id) =>
        _db.Payments
            .Include(x => x.Booking)
                .ThenInclude(x => x!.Event)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task AddAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
