namespace EventParkingReservationSystem.Models;

public class ParkingReservation
{
    public int Id { get; set; }
<<<<<<< HEAD
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public int ParkingSlotId { get; set; }
    public ParkingSlot? ParkingSlot { get; set; }
    public decimal ReservedFee { get; set; }
=======

    public int BookingId { get; set; }

    public int ParkingSlotId { get; set; }

    public int EventId { get; set; }

    public decimal ReservedFee { get; set; }


    public Booking? Booking { get; set; }

    public ParkingSlot? ParkingSlot { get; set; }

    public Event? Event { get; set; }
}
//     public int BookingId { get; set; }
//     public Booking? Booking { get; set; }
//     public int ParkingSlotId { get; set; }
//     public ParkingSlot? ParkingSlot { get; set; }
//     public decimal ReservedFee { get; set; }
>>>>>>> origin/master
}
