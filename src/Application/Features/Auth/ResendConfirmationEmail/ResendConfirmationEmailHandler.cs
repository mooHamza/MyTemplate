

using FullTemplate.Application.Common.Interfaces;

namespace FullTemplate.Application.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailHandler
(
    IAuthService _authService
) : IRequestHandler<ResendConfirmationEmailCommand>
{
    public async Task Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        await _authService.ResendConfirmationEmailAsync(request.Email);
    }
}
