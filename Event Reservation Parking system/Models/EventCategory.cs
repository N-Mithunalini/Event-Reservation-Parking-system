using System.Collections.Generic;

namespace EventParkingReservationSystem.Models;

public class EventCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
