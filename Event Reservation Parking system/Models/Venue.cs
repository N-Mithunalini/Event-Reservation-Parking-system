using System.Collections.Generic;

namespace EventParkingReservationSystem.Models;

public class Venue
{
    public int Id { get; set; }
<<<<<<< HEAD
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
=======

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public ICollection<Event> Events { get; set; }
        = new List<Event>();
}
>>>>>>> origin/master
