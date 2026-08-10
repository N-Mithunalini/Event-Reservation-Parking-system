namespace EventParkingReservationSystem.Models;

public class Seat
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string Row { get; set; } = string.Empty;

    public int Column { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public string SeatType { get; set; } = "Standard";

    public string Status { get; set; } = "Available";

    public Event? Event { get; set; }

    public ICollection<BookingSeat> BookingSeats { get; set; }
        = new List<BookingSeat>();
}