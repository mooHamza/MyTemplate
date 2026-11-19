

using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Application.Common.Results;

namespace FullTemplate.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    IAuthService _authService
) : IRequestHandler<ConfirmEmailCommand, Result>
{


    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmailAsync(request.UserId, request.Code);
        return result;
    }
}
