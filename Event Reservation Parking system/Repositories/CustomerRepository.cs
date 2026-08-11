using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _db;

    public CustomerRepository(ApplicationDbContext db)
    {
        _db = db;
    }


    public async Task<List<Customer>> GetAllAsync(
        string? search = null)
    {
        var query = _db.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Email.Contains(search)
            );
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync();
    }


    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _db.Customers
            .Include(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<Customer?> GetByEmailAsync(
        string email)
    {
        return await _db.Customers
            .FirstOrDefaultAsync(x =>
                x.Email == email);
    }


    public async Task<Customer?>
        GetByVerificationTokenAsync(string token)
    {
        return await _db.Customers
            .FirstOrDefaultAsync(x =>
                x.EmailVerificationToken == token);
    }


    public async Task<Customer?>
        GetByResetTokenAsync(string token)
    {
        return await _db.Customers
            .FirstOrDefaultAsync(x =>
                x.PasswordResetToken == token);
    }


    public async Task<bool> EmailExistsAsync(
        string email)
    {
        return await _db.Customers
            .AnyAsync(x => x.Email == email);
    }


    public async Task<bool> HasActiveBookingsAsync(
        int customerId)
    {
        return await _db.Bookings
            .AnyAsync(x =>
                x.CustomerId == customerId &&
                (
                    x.Status == "Pending" ||
                    x.Status == "Confirmed"
                )
            );
    }


    public async Task AddAsync(Customer customer)
    {
        await _db.Customers.AddAsync(customer);
    }


    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}