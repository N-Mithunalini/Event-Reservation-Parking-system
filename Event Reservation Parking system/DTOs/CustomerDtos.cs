namespace EventParkingReservationSystem.DTOs;

public record CustomerUpdateDto
{
    public string Name { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;
}