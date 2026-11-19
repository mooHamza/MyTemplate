using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Application.Common.Results;
using FullTemplate.Application.Features.Auth.Login.Commands;

namespace FullTemplate.Application.Features.Auth.Refresh.Commands;

public class RefreshTokenCommandHandler
(
    IAuthService _authService
) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken, cancellationToken);
        return result;
    }
}
