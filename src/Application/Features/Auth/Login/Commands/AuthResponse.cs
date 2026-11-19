using System.Text.Json.Serialization;

namespace FullTemplate.Application.Features.Auth.Login.Commands;

public record AuthResponse(string TokenType, string AccessToken, int ExpiresIn, string RefreshToken)
{
    /// <summary>Used by the controller for cookie expiry; not sent to the client.</summary>
    [JsonIgnore]
    public DateTimeOffset RefreshTokenExpiresAt { get; init; }
}
