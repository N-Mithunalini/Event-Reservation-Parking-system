namespace EventParkingReservationSystem.DTOs;

public record EventDto(
    string Name, int VenueId, int CategoryId, DateTime EventDate,
    TimeSpan StartTime, TimeSpan EndTime, decimal TicketPrice,
    decimal ParkingFee, int Capacity);
