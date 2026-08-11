using System;

<<<<<<< Updated upstream
=======
namespace EventParkingReservationSystem.DTOs;
>>>>>>> Stashed changes
public record EventDto(
    string Name, int VenueId, int CategoryId, DateTime EventDate,
    TimeSpan StartTime, TimeSpan EndTime, decimal TicketPrice,
    decimal ParkingFee, int Capacity);
