namespace EventParkingReservationSystem.Models;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
