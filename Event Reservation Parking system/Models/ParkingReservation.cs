namespace EventParkingReservationSystem.Models;

public class ParkingReservation
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public int ParkingSlotId { get; set; }
    public ParkingSlot? ParkingSlot { get; set; }
    public decimal ReservedFee { get; set; }

}
