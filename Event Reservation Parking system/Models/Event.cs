namespace EventParkingReservationSystem.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int VenueId { get; set; }
    public Venue? Venue { get; set; }
    public int CategoryId { get; set; }
    public EventCategory? Category { get; set; }
    public DateTime EventDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal TicketPrice { get; set; }
    public decimal ParkingFee { get; set; }
    public int Capacity { get; set; }
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

