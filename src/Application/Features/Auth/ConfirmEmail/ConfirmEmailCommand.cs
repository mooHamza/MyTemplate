using FullTemplate.Application.Common.Results;

namespace FullTemplate.Application.Features.Auth.ConfirmEmail;


public record ConfirmEmailCommand(string UserId, string Code) : IRequest<Result>;
