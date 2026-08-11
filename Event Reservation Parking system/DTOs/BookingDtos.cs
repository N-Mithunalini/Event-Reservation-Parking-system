using System.Collections.Generic;

namespace EventParkingReservationSystem.DTOs;
public record BookingCreateDto(int CustomerId, int EventId, List<int> SeatIds, int? ParkingSlotId);
