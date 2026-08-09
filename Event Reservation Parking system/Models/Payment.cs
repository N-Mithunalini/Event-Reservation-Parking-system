namespace EventParkingReservationSystem.Models;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Completed";
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}
