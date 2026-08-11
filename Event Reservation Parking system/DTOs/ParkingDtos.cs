namespace EventParkingReservationSystem.DTOs;

public record ParkingLayoutCreateDto(int SlotCount, string? Zone, decimal Fee);
public record ParkingSlotUpdateDto(string SlotNumber, string? Zone, decimal Fee, string Status);

}
