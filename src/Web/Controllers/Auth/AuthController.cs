using FullTemplate.Application.Features.Auth.Login.Commands;
using Microsoft.AspNetCore.Mvc;
using FullTemplate.Application.Features.Auth.Register.Commands;
using FullTemplate.Application.Features.Auth.Refresh.Commands;
using FullTemplate.Application.Features.Auth.ForgotPassword.Commands;
using FullTemplate.Application.Features.Auth.ResetPassword;
using FullTemplate.Application.Features.Auth.ConfirmEmail;
using FullTemplate.Application.Features.Auth.LogOut;
using FullTemplate.Application.Features.Auth.ResendConfirmationEmail;
namespace FullTemplate.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController
(
    IMediator _mediator
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterCommand request)
    {
        var result = await _mediator.Send(request);
        return result.Succeeded 
        ? NoContent()
        : result.ToFailure();
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCommand request)
    {
        var result = await _mediator.Send(request);
        return result.Succeeded 
        ? Ok(result.Value) 
        : result.ToFailure();
    }
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        RefreshTokenCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);

        return result.Succeeded
            ? Ok(result.Value)
            : result.ToFailure();
    }

    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }
    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand request)
    {
        var result = await _mediator.Send(request);
        return result.Succeeded
            ? Ok()
            : result.ToFailure();
    }

    [HttpPost("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand request)
    {
        var result = await _mediator.Send(request);
        return result.Succeeded
            ? Ok()
            : result.ToFailure();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogOutCommand request)
    {
        await _mediator.Send(request);
        return NoContent();
    }

    [HttpPost("resendConfirmationEmail")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailCommand request)
    {
        await _mediator.Send(request);
        return NoContent();
    }
}