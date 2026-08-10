namespace EventParkingReservationSystem.Models;

public class Booking
{
    internal int EventId;

    public string Status { get; internal set; }
    public object CustomerId { get; internal set; }
    public object? BookingNumber { get; internal set; }
    public ParkingReservation? ParkingReservation { get; set; }
    public Payment? Payment { get; set; }

}
