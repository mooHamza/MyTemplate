

using FullTemplate.Application.Common.Interfaces;

namespace FullTemplate.Application.Features.Auth.ForgotPassword.Commands;

public class ForgotPasswordCommandHandler
(
    IAuthService _authService
) : IRequestHandler<ForgotPasswordCommand>
{


    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        await _authService.ForgotPasswordAsync(request.Email, cancellationToken);
         
    }
}
