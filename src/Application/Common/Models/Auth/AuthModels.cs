using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FullTemplate.Application.Models.Auth;

public record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

/// <summary>RefreshToken may be null when the client uses cookies.</summary>
public record RefreshRequest(string? RefreshToken);

public record ResendConfirmationRequest([property: Required, EmailAddress] string Email);

public record ForgotPasswordRequest([property: Required, EmailAddress] string Email);

public record ResetPasswordRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string ResetCode,
    [property: Required] string NewPassword);



