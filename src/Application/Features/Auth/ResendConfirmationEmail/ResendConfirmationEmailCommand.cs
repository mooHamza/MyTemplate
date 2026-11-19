namespace FullTemplate.Application.Features.Auth.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email) : IRequest;
