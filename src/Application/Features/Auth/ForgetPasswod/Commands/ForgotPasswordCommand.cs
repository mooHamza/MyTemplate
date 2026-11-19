namespace FullTemplate.Application.Features.Auth.ForgotPassword.Commands;
public record ForgotPasswordCommand(string Email) : IRequest;
