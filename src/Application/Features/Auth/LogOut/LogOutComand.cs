

using FullTemplate.Application.Common.Interfaces;

namespace FullTemplate.Application.Features.Auth.LogOut;

public record LogOutCommand(string refreshToken) : IRequest;

public class LogOutValidator : AbstractValidator<LogOutCommand>
{
    public LogOutValidator()
    {
        RuleFor(x => x.refreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}


public class LogOutHandler
(

      IAuthService _authService
) : IRequestHandler<LogOutCommand>
{
 
    public async Task Handle(LogOutCommand request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request.refreshToken, cancellationToken);
    }
}