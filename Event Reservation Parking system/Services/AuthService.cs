using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;

using Microsoft.IdentityModel.Tokens;

namespace EventParkingReservationSystem.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customers;
    private readonly IConfiguration _configuration;

    public AuthService(
        ICustomerRepository customers,
        IConfiguration configuration)
    {
        _customers = customers;
        _configuration = configuration;
    }


    public async Task<object> RegisterAsync(
        RegisterDto dto)
    {
        var email =
            dto.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new InvalidOperationException(
                "Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException(
                "Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password) ||
            dto.Password.Length < 6)
        {
            throw new InvalidOperationException(
                "Password must contain at least 6 characters.");
        }

        if (await _customers.EmailExistsAsync(email))
        {
            throw new InvalidOperationException(
                "Email already registered.");
        }

        var verificationToken =
            GenerateToken();

        var customer = new Customer
        {
            Name = dto.Name.Trim(),

            Email = email,

            Phone = dto.Phone.Trim(),

            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.Password),

            Role = "Customer",

            Status = "Active",

            EmailVerified = false,

            EmailVerificationToken =
                verificationToken,

            EmailVerificationTokenExpiresAt =
                DateTime.UtcNow.AddHours(24),

            CreatedAt = DateTime.UtcNow
        };

        await _customers.AddAsync(customer);
        await _customers.SaveAsync();

        return new
        {
            message =
                "Registration successful.",

            customerId =
                customer.Id,

            verificationToken
        };
    }


    public async Task<object> LoginAsync(
        LoginDto dto)
    {
        var email =
            dto.Email.Trim().ToLower();

        var customer =
            await _customers.GetByEmailAsync(email);

        if (customer == null ||
            !BCrypt.Net.BCrypt.Verify(
                dto.Password,
                customer.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (customer.Status != "Active")
        {
            throw new UnauthorizedAccessException(
                "Customer account is deactivated.");
        }

        if (!customer.EmailVerified)
        {
            throw new UnauthorizedAccessException(
                "Please verify your email before login.");
        }

        var token =
            GenerateJwt(customer);

        return new
        {
            id = customer.Id,
            name = customer.Name,
            email = customer.Email,
            role = customer.Role,
            token
        };
    }


    public async Task<object> VerifyEmailAsync(
        string token)
    {
        var customer =
            await _customers
                .GetByVerificationTokenAsync(token);

        if (customer == null)
        {
            throw new InvalidOperationException(
                "Invalid verification token.");
        }

        if (!customer
                .EmailVerificationTokenExpiresAt
                .HasValue ||
            customer
                .EmailVerificationTokenExpiresAt
                .Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Verification token has expired.");
        }

        customer.EmailVerified = true;

        customer.EmailVerificationToken = null;

        customer.EmailVerificationTokenExpiresAt =
            null;

        await _customers.SaveAsync();

        return new
        {
            message =
                "Email verified successfully."
        };
    }


    public async Task<object>
        ResendVerificationAsync(
            ResendVerificationDto dto)
    {
        var email =
            dto.Email.Trim().ToLower();

        var customer =
            await _customers.GetByEmailAsync(email);

        if (customer == null)
        {
            return new
            {
                message =
                    "If the account exists, a verification token has been generated."
            };
        }

        if (customer.EmailVerified)
        {
            return new
            {
                message =
                    "Email is already verified."
            };
        }

        var token = GenerateToken();

        customer.EmailVerificationToken = token;

        customer.EmailVerificationTokenExpiresAt =
            DateTime.UtcNow.AddHours(24);

        await _customers.SaveAsync();

        return new
        {
            message =
                "Verification token generated.",

            verificationToken = token
        };
    }


    public async Task<object> ForgotPasswordAsync(
        ForgotPasswordDto dto)
    {
        var email =
            dto.Email.Trim().ToLower();

        var customer =
            await _customers.GetByEmailAsync(email);

        if (customer == null)
        {
            return new
            {
                message =
                    "If the email exists, a reset token has been generated."
            };
        }

        var resetToken =
            GenerateToken();

        customer.PasswordResetToken =
            resetToken;

        customer.PasswordResetTokenExpiresAt =
            DateTime.UtcNow.AddMinutes(60);

        await _customers.SaveAsync();

        return new
        {
            message =
                "Password reset token generated.",

            resetToken
        };
    }


    public async Task<object> ResetPasswordAsync(
        ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(
                dto.NewPassword) ||
            dto.NewPassword.Length < 6)
        {
            throw new InvalidOperationException(
                "New password must contain at least 6 characters.");
        }

        var customer =
            await _customers
                .GetByResetTokenAsync(dto.Token);

        if (customer == null)
        {
            throw new InvalidOperationException(
                "Invalid reset token.");
        }

        if (!customer
                .PasswordResetTokenExpiresAt
                .HasValue ||
            customer
                .PasswordResetTokenExpiresAt
                .Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Reset token has expired.");
        }

        customer.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword);

        customer.PasswordResetToken = null;

        customer.PasswordResetTokenExpiresAt =
            null;

        await _customers.SaveAsync();

        return new
        {
            message =
                "Password reset successfully."
        };
    }


    private string GenerateJwt(
        Customer customer)
    {
        var key =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "Jwt:Key missing.");

        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    customer.Id.ToString()
                ),

                new(
                    ClaimTypes.Name,
                    customer.Name
                ),

                new(
                    ClaimTypes.Email,
                    customer.Email
                ),

                new(
                    ClaimTypes.Role,
                    customer.Role
                )
            };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var jwt =
            new JwtSecurityToken(
                issuer:
                    _configuration["Jwt:Issuer"],

                audience:
                    _configuration["Jwt:Audience"],

                claims: claims,

                expires:
                    DateTime.UtcNow.AddHours(8),

                signingCredentials:
                    credentials
            );

        return new JwtSecurityTokenHandler()
            .WriteToken(jwt);
    }


    private static string GenerateToken()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));
    }
}