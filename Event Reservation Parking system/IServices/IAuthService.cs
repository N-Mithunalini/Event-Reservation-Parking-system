using EventParkingReservationSystem.DTOs;

namespace EventParkingReservationSystem.IServices;

public interface IAuthService
{
    Task<object> RegisterAsync(RegisterDto dto);

    Task<object> LoginAsync(LoginDto dto);

    Task<object> VerifyEmailAsync(string token);

    Task<object> ResendVerificationAsync(
        ResendVerificationDto dto);

    Task<object> ForgotPasswordAsync(
        ForgotPasswordDto dto);

    Task<object> ResetPasswordAsync(
        ResetPasswordDto dto);
}