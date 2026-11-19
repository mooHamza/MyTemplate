using FullTemplate.Application.Common.Results;

namespace FullTemplate.Application.Features.Auth.ResetPassword;

public record ResetPasswordCommand(string Email, string ResetCode, string NewPassword) : IRequest<Result>;
