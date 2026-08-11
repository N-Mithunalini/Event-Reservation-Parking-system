using System.Data;
using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _db;
    private readonly IBookingRepository _bookings;
    private readonly IPaymentRepository _payments;
    private readonly INotificationService _notifications;

    public PaymentService(
        ApplicationDbContext db,
        IBookingRepository bookings,
        IPaymentRepository payments,
        INotificationService notifications)
    {
        _db = db;
        _bookings = bookings;
        _payments = payments;
        _notifications = notifications;
    }

    private static decimal Total(Booking booking)
    {
        var ticket = booking.Event?.TicketPrice ?? 0m;
        var seatTotal = booking.BookingSeats.Count * ticket;
        var parking = booking.ParkingReservation?.ReservedFee ?? 0m;
        return seatTotal + parking;
    }

    public async Task<object> GetForBookingAsync(int bookingId)
    {
        var booking = await _bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found.");
        var payment = await _payments.GetByBookingAsync(bookingId);

        return new
        {
            amountDue = Total(booking),
            paymentStatus = payment?.Status ?? "Unpaid"
        };
    }

    public async Task<object> PayAsync(int bookingId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var booking = await _bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found.");

        if (booking.Status == "Expired")
            throw new InvalidOperationException("Booking hold has expired. Create a new booking.");
        if (booking.Status != "Pending")
            throw new InvalidOperationException("Only pending bookings can be paid.");
        if (!booking.HoldExpiresAt.HasValue || booking.HoldExpiresAt.Value <= DateTime.UtcNow)
            throw new InvalidOperationException("Booking hold has expired. Create a new booking.");
        if (await _payments.GetByBookingAsync(bookingId) is not null)
            throw new ConflictException("Payment already exists for this booking.");

        var payment = new Payment
        {
            BookingId = bookingId,
            Amount = Total(booking),
            Status = "Completed",
            PaidAt = DateTime.UtcNow
        };

        await _payments.AddAsync(payment);
        booking.Status = "Confirmed";
        booking.HoldExpiresAt = null;

        foreach (var bookingSeat in booking.BookingSeats)
            if (bookingSeat.Seat is not null)
                bookingSeat.Seat.Status = "Booked";

        if (booking.ParkingReservation?.ParkingSlot is not null)
            booking.ParkingReservation.ParkingSlot.Status = "Reserved";

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        await _notifications.CreateAsync(
            booking.CustomerId,
            $"Payment completed. Booking {booking.BookingNumber} is confirmed.");

        return new
        {
            paymentId = payment.Id,
            payment.Amount,
            payment.Status,
            payment.PaidAt,
            bookingId = booking.Id,
            bookingNumber = booking.BookingNumber
        };
    }

    public async Task<object> GetByCustomerAsync(int customerId)
    {
        var rows = await _payments.GetByCustomerAsync(customerId);
        return rows.Select(payment => new
        {
            payment.Id,
            payment.BookingId,
            payment.Amount,
            payment.Status,
            payment.PaidAt,
            booking = payment.Booking is null ? null : new
            {
                payment.Booking.BookingNumber,
                eventName = payment.Booking.Event?.Name
            }
        }).ToList();
    }

    public async Task<object> GetReceiptAsync(int paymentId)
    {
        var payment = await _payments.GetByIdAsync(paymentId)
            ?? throw new KeyNotFoundException("Payment not found.");

        return new
        {
            receiptNumber = $"RCT-{payment.Id:000000}",
            payment.Id,
            payment.BookingId,
            payment.Amount,
            payment.Status,
            payment.PaidAt,
            bookingNumber = payment.Booking?.BookingNumber,
            eventName = payment.Booking?.Event?.Name
        };
    }
}
