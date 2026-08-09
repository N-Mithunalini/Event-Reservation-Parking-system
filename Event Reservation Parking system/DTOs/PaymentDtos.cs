namespace EventParkingReservationSystem.DTOs;
public record PaymentResultDto(int PaymentId, decimal Amount, string Status, DateTime PaidAt);
