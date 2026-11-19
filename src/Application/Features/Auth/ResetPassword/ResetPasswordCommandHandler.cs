

using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Application.Common.Results;

namespace FullTemplate.Application.Features.Auth.ResetPassword;

public class ResetPasswordCommandHandler
(
    IAuthService _authService
) : IRequestHandler<ResetPasswordCommand, Result>
{

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request.Email, request.ResetCode, request.NewPassword, cancellationToken);
        return result;
    }
}
