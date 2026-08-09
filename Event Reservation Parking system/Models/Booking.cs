namespace EventParkingReservationSystem.Models;

public class Booking
{
    public int Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? HoldExpiresAt { get; set; }
    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    public ParkingReservation? ParkingReservation { get; set; }
    public Payment? Payment { get; set; }
}
