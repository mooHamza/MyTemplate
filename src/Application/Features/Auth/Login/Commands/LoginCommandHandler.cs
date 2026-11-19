using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Application.Common.Results;
namespace FullTemplate.Application.Features.Auth.Login.Commands;


public class LoginCommandHandler
(
    IAuthService _authService
) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return result;
    }
}