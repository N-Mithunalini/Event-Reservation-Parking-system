namespace EventParkingReservationSystem.Models;

public class BookingSeat
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int SeatId { get; set; }

    public int EventId { get; set; }


    public Booking? Booking { get; set; }

    public Seat? Seat { get; set; }

    public Event? Event { get; set; }
}
//     public int BookingId { get; set; }
//     public Booking? Booking { get; set; }
//     public int SeatId { get; set; }
//     public Seat? Seat { get; set; }
}
