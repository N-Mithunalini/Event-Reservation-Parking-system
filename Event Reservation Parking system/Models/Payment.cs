namespace EventParkingReservationSystem.Models;

public class Payment
{
    public object? BookingId { get; internal set; }
    public object Amount { get; internal set; }
    public object Booking { get; internal set; }
}
