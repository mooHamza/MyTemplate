using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Application.Common.Results;

namespace FullTemplate.Application.Features.Auth.Register.Commands;

public class RegisterCommandHandler
(
    IAuthService _authService
) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request.Email, request.Password);
        return result;
    }
}