
 using FullTemplate.Application.Common.Results;


namespace FullTemplate.Application.Features.Auth.Login.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
