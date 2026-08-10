namespace EventParkingReservationSystem.Models;

public class ParkingSlot
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string SlotNumber { get; set; } = string.Empty;

    public string Zone { get; set; } = string.Empty;

    public decimal Fee { get; set; }

    public string Status { get; set; } = "Available";

    public Event? Event { get; set; }

    public ICollection<ParkingReservation> ParkingReservations { get; set; }
        = new List<ParkingReservation>();
}