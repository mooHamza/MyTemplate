using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FullTemplate.Domain.Entities;
using FullTemplate.Infrastructure.Data;
using FullTemplate.Infrastructure.Identity.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FullTemplate.Application.Features.Auth.Login.Commands;
using FullTemplate.Application.Common.Results;

namespace FullTemplate.Infrastructure.Identity;

internal sealed class TokenService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext db,
    IOptions<JwtOptions> options) 
{
    private readonly JwtOptions _jwt = options.Value;

    /// <summary>
    /// Creates an access token (JWT) and a new refresh token. If <paramref name="replacing"/> is given,
    /// that token is revoked and linked to the new one (rotation) in the same SaveChanges.
    /// </summary>
    public async Task<AuthResponse> IssueAsync(
        ApplicationUser user, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        // ----- access token (JWT) -----
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(_jwt.AccessTokenMinutes).UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var (refreshToken, refreshEntity) = GenerateRefreshToken(user.Id, now);
        
        db.RefreshTokens.Add(refreshEntity);

        await db.SaveChangesAsync(ct);

        return new AuthResponse("Bearer", accessToken, _jwt.AccessTokenMinutes * 60, refreshToken)
        {
            RefreshTokenExpiresAt = refreshEntity.ExpiresAt
        };
    }

  public async Task<Result<AuthResponse>> RefreshAsync(
        string refreshToken,
        CancellationToken ct)
    {
        var hash = Hash(refreshToken);

        var stored = await db.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (stored is null ||
            stored.RevokedAt != null ||
            stored.ExpiresAt <= DateTimeOffset.UtcNow)
            return Result<AuthResponse>.Failure(ErrorKind.Validation, "Invalid refresh token.");

        var user = await userManager.FindByIdAsync(stored.UserId);

        if (user is null)
            return Result<AuthResponse>.Failure(ErrorKind.Validation, "User not found." );

        // Revoke old token
        stored.RevokedAt = DateTimeOffset.UtcNow;

        // Create new tokens
        var result = await IssueAsync(user, ct);

        await db.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(result);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken ct)
    {
        var hash = Hash(refreshToken);
        var stored = await db.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now), ct);
    }
    private (string Token, RefreshToken Entity) GenerateRefreshToken(string userId, DateTimeOffset now)
{
    var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

    var entity = new RefreshToken
    {
        UserId = userId,
        TokenHash = Hash(token),
        CreatedAt = now,
        ExpiresAt = now.AddDays(_jwt.RefreshTokenDays),
    };

    return (token, entity);
}

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
