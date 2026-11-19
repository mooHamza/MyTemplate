using System.Text;

using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Application.Common.Results;
using FullTemplate.Application.Features.Auth.Login.Commands;
using FullTemplate.Infrastructure.Identity.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FullTemplate.Infrastructure.Identity;

internal sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    TokenService tokenService,
    IEmailSender<ApplicationUser> emailSender,
    IOptions<AuthLinkOptions> linkOptions) : IAuthService
{
    private readonly AuthLinkOptions _links = linkOptions.Value;

    // ---------------------------------------------------------------- register
    public async Task<Result> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return result.ToApplicationResult();

        await SendConfirmationEmailAsync(user);
        return Result.Success();
    }

    // ------------------------------------------------------------------- login
    public async Task<Result<AuthResponse>> LoginAsync(
        string email, string password, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) 
        return Result<AuthResponse>.Failure(ErrorKind.Validation, "Invalid credentials.");

        
        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded) 
            return result.ToApplicationResult<AuthResponse>();

        var tokens = await tokenService.IssueAsync(user, ct);
        return Result<AuthResponse>.Success(tokens);
    }

    // ----------------------------------------------------------------- refresh
    public Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct)
        => tokenService.RefreshAsync(refreshToken, ct);

    // ------------------------------------------------------------ confirm email
    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null || !TryDecode(code, out var token))
            return Result.Failure(ErrorKind.Validation, "Invalid confirmation link." );

        var result = await userManager.ConfirmEmailAsync(user, token);
        return result.ToApplicationResult();
    }

    public async Task ResendConfirmationEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        // Silent when the user doesn't exist or is already confirmed.
        if (user is null || await userManager.IsEmailConfirmedAsync(user)) return;

        await SendConfirmationEmailAsync(user);
    }

    // ---------------------------------------------------------- forgot / reset
    public async Task ForgotPasswordAsync(string email, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);

        // Silent when the user doesn't exist or isn't confirmed.
        if (user is null || !await userManager.IsEmailConfirmedAsync(user)) return;

        var code = await userManager.GeneratePasswordResetTokenAsync(user);

        await emailSender.SendPasswordResetCodeAsync(user, email, code);
    }

    public async Task<Result> ResetPasswordAsync(
        string email, string resetCode, string newPassword, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);

        // Same generic error whether the user or the code is wrong.
        if (user is null )
            return Result.Failure(ErrorKind.Validation, "Invalid reset code." );

        var result = await userManager.ResetPasswordAsync(user, resetCode, newPassword);
        if (!result.Succeeded) return result.ToApplicationResult();

        // Kill every existing session after a password reset.
        // when Access token expire (usually short time) user must login again with new pasword
        // revoke all access token, because user may login from many devices (lap, mobile, pc) 
        // each login create refresh token , here we kill all 
        await tokenService.RevokeAllForUserAsync(user.Id, ct);
        return Result.Success();
    }

    // ------------------------------------------------------------------ logout
    
    public Task LogoutAsync(string refreshToken, CancellationToken ct)
        => tokenService.RevokeAsync(refreshToken, ct);
        // when access token expire (usually short time ) user should login again
        // front-end remove Access token from ......

    // ----------------------------------------------------------------- helpers
    private async Task SendConfirmationEmailAsync(ApplicationUser user)
    {
        var code = Encode(await userManager.GenerateEmailConfirmationTokenAsync(user));
        var link = $"{_links.ConfirmEmailUrl}?userId={Uri.EscapeDataString(user.Id)}&code={code}";

        await emailSender.SendConfirmationLinkAsync(user, user.Email!, link);
    }

    private static string Encode(string token)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    private static bool TryDecode(string code, out string token)
    {
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            return true;
        }
        catch (FormatException)
        {
            token = string.Empty;
            return false;
        }
    }
}
