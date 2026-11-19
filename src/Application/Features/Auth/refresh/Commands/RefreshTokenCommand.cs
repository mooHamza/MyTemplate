using FullTemplate.Application.Common.Results;
using FullTemplate.Application.Features.Auth.Login.Commands;

namespace FullTemplate.Application.Features.Auth.Refresh.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
    