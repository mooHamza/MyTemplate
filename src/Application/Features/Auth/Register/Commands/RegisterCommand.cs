using FullTemplate.Application.Common.Results;

namespace FullTemplate.Application.Features.Auth.Register.Commands;

public record RegisterCommand(string Email, string Password) : IRequest<Result>;