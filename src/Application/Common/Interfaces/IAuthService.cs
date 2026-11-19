
using FullTemplate.Application.Common.Results;
using FullTemplate.Application.Features.Auth.Login.Commands;

namespace FullTemplate.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(string email, string password);

    Task<Result<AuthResponse>> LoginAsync(
        string email, string password, CancellationToken ct);

    /// <summary>Returns null when the refresh token is invalid, expired or revoked.</summary>
    Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct);

    Task<Result> ConfirmEmailAsync(string userId, string code);

    Task ResendConfirmationEmailAsync(string email);

    Task ForgotPasswordAsync(string email, CancellationToken ct);

    Task<Result> ResetPasswordAsync(string email, string resetCode, string newPassword, CancellationToken ct);

    Task LogoutAsync(string refreshToken, CancellationToken ct);
}
