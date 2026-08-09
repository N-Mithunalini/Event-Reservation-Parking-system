using System.Data;
using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _db;
    private readonly IBookingRepository _bookings;
    private readonly ICustomerRepository _customers;
    private readonly INotificationService _notifications;
    private readonly IConfiguration _config;

    public BookingService(
        ApplicationDbContext db,
        IBookingRepository bookings,
        ICustomerRepository customers,
        INotificationService notifications,
        IConfiguration config)
    {
        _db = db;
        _bookings = bookings;
        _customers = customers;
        _notifications = notifications;
        _config = config;
    }

    private static object MapBooking(Booking booking) => new
    {
        booking.Id,
        booking.BookingNumber,
        booking.CustomerId,
        booking.EventId,
        booking.Status,
        booking.CreatedAt,
        booking.HoldExpiresAt,
        customer = booking.Customer is null ? null : new
        {
            booking.Customer.Id,
            booking.Customer.Name,
            booking.Customer.Email
        },
        @event = booking.Event is null ? null : new
        {
            booking.Event.Id,
            booking.Event.Name,
            booking.Event.EventDate,
            booking.Event.StartTime,
            booking.Event.EndTime,
            booking.Event.TicketPrice
        },
        bookingSeats = booking.BookingSeats.Select(bs => new
        {
            bs.Id,
            bs.SeatId,
            seat = bs.Seat is null ? null : new
            {
                bs.Seat.Id,
                bs.Seat.SeatNumber,
                bs.Seat.Status,
                bs.Seat.SeatType
            }
        }).ToList(),
        parkingReservation = booking.ParkingReservation is null ? null : new
        {
            booking.ParkingReservation.Id,
            booking.ParkingReservation.ParkingSlotId,
            booking.ParkingReservation.ReservedFee,
            parkingSlot = booking.ParkingReservation.ParkingSlot is null ? null : new
            {
                booking.ParkingReservation.ParkingSlot.Id,
                booking.ParkingReservation.ParkingSlot.SlotNumber,
                booking.ParkingReservation.ParkingSlot.Zone,
                booking.ParkingReservation.ParkingSlot.Fee,
                booking.ParkingReservation.ParkingSlot.Status
            }
        },
        payment = booking.Payment is null ? null : new
        {
            booking.Payment.Id,
            booking.Payment.Amount,
            booking.Payment.Status,
            booking.Payment.PaidAt
        }
    };

    public async Task<object> CreateAsync(BookingCreateDto dto)
    {
        if (dto.SeatIds is null || dto.SeatIds.Count == 0)
            throw new InvalidOperationException("At least one seat is required.");
        if (dto.SeatIds.Distinct().Count() != dto.SeatIds.Count)
            throw new InvalidOperationException("Duplicate seats are not allowed.");

        var customer = await _customers.GetByIdAsync(dto.CustomerId)
            ?? throw new InvalidOperationException("Customer not found.");

        if (customer.Status != "Active")
            throw new UnauthorizedAccessException("Customer account is deactivated.");
        if (!customer.EmailVerified)
            throw new UnauthorizedAccessException("Customer email must be verified before booking.");

        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var eventExists = await _db.Events.AnyAsync(e => e.Id == dto.EventId);
        if (!eventExists)
            throw new KeyNotFoundException("Event not found.");

        var seats = await _db.Seats
            .Where(s => dto.SeatIds.Contains(s.Id) && s.EventId == dto.EventId)
            .ToListAsync();

        if (seats.Count != dto.SeatIds.Count)
            throw new InvalidOperationException("One or more selected seats are invalid for this event.");
        if (seats.Any(s => s.Status != "Available"))
            throw new ConflictException("One or more seats were just taken. Refresh the seat map and choose again.");

        ParkingSlot? slot = null;
        if (dto.ParkingSlotId.HasValue)
        {
            slot = await _db.ParkingSlots
                .FirstOrDefaultAsync(p => p.Id == dto.ParkingSlotId.Value && p.EventId == dto.EventId)
                ?? throw new InvalidOperationException("Parking slot not found for this event.");

            if (slot.Status != "Available")
                throw new ConflictException("The parking slot was just taken. Refresh and choose another slot.");
        }

        var holdMinutes = _config.GetValue<int?>("BookingHoldMinutes") ?? 15;
        var booking = new Booking
        {
            BookingNumber = $"BKG-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            CustomerId = dto.CustomerId,
            EventId = dto.EventId,
            Status = "Pending",
            HoldExpiresAt = DateTime.UtcNow.AddMinutes(holdMinutes)
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        foreach (var seat in seats)
        {
            seat.Status = "Held";
            _db.BookingSeats.Add(new BookingSeat
            {
                BookingId = booking.Id,
                SeatId = seat.Id
            });
        }

        if (slot is not null)
        {
            slot.Status = "Held";
            _db.ParkingReservations.Add(new ParkingReservation
            {
                BookingId = booking.Id,
                ParkingSlotId = slot.Id,
                ReservedFee = slot.Fee
            });
        }

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        await _notifications.CreateAsync(
            dto.CustomerId,
            $"Booking {booking.BookingNumber} created. Complete payment before the hold expires.");

        var created = await _bookings.GetByIdAsync(booking.Id) ?? booking;
        return MapBooking(created);
    }

    public async Task<object> GetByIdAsync(int id)
    {
        var booking = await _bookings.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Booking not found.");
        return MapBooking(booking);
    }

    public async Task<object> GetByCustomerAsync(int customerId)
    {
        var rows = await _bookings.GetByCustomerAsync(customerId);
        return rows.Select(MapBooking).ToList();
    }

    public async Task<object> GetByEventAsync(int eventId)
    {
        var rows = await _bookings.GetByEventAsync(eventId);
        return rows.Select(MapBooking).ToList();
    }

    public async Task<object> CancelAsync(int id)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var booking = await _bookings.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Booking not found.");

        if (booking.Status is "Cancelled" or "Expired")
            return new { message = "Booking is already closed." };

        booking.Status = "Cancelled";
        booking.HoldExpiresAt = null;

        foreach (var bookingSeat in booking.BookingSeats)
            if (bookingSeat.Seat is not null)
                bookingSeat.Seat.Status = "Available";

        if (booking.ParkingReservation?.ParkingSlot is not null)
            booking.ParkingReservation.ParkingSlot.Status = "Available";

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        await _notifications.CreateAsync(
            booking.CustomerId,
            $"Booking {booking.BookingNumber} was cancelled.");

        return new { message = "Booking cancelled and reserved resources released." };
    }

    public async Task<object> HoldStatusAsync(int id)
    {
        var booking = await _bookings.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Booking not found.");

        var remaining = booking.HoldExpiresAt.HasValue
            ? booking.HoldExpiresAt.Value - DateTime.UtcNow
            : TimeSpan.Zero;

        return new
        {
            booking.Status,
            booking.HoldExpiresAt,
            remainingSeconds = Math.Max(0, (int)remaining.TotalSeconds)
        };
    }
}
