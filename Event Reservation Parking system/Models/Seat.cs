namespace EventParkingReservationSystem.Models;

public class Seat
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Column { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public string? SeatType { get; set; }

}
