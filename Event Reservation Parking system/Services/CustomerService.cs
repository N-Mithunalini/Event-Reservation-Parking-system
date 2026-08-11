using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;

namespace EventParkingReservationSystem.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customers;

    public CustomerService(
        ICustomerRepository customers)
    {
        _customers = customers;
    }


    public async Task<object> GetAllAsync(
        string? search)
    {
        var customers =
            await _customers.GetAllAsync(search);

        return customers.Select(x => new
        {
            x.Id,
            x.Name,
            x.Email,
            x.Phone,
            x.Role,
            x.Status,
            x.EmailVerified,
            x.CreatedAt
        }).ToList();
    }


    public async Task<object> GetByIdAsync(int id)
    {
        var customer =
            await _customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                "Customer not found.");

        return new
        {
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.Role,
            customer.Status,
            customer.EmailVerified,
            customer.CreatedAt,

            TotalBookings =
                customer.Bookings.Count
        };
    }


    public async Task<object> UpdateAsync(
        int id,
        CustomerUpdateDto dto)
    {
        var customer =
            await _customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                "Customer not found.");

        customer.Name = dto.Name.Trim();
        customer.Phone = dto.Phone.Trim();

        await _customers.SaveAsync();

        return new
        {
            message =
                "Customer updated successfully."
        };
    }


    public async Task<object> DeactivateAsync(int id)
    {
        var customer =
            await _customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                "Customer not found.");

        if (await _customers
            .HasActiveBookingsAsync(id))
        {
            throw new InvalidOperationException(
                "Customer has an active booking and cannot be deactivated."
            );
        }

        customer.Status = "Deactivated";

        await _customers.SaveAsync();

        return new
        {
            message =
                "Customer deactivated successfully."
        };
    }


    public async Task<object> ReactivateAsync(int id)
    {
        var customer =
            await _customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                "Customer not found.");

        customer.Status = "Active";

        await _customers.SaveAsync();

        return new
        {
            message =
                "Customer reactivated successfully."
        };
    }
}