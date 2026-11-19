namespace FullTemplate.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string UserId { get; set; } = default!;

    public string TokenHash { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>Hash of the token that replaced this one (set on rotation).</summary>
    public string? ReplacedByTokenHash { get; set; }

    public string? CreatedByIp { get; set; }
}
