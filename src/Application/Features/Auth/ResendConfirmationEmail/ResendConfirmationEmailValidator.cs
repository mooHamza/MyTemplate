namespace FullTemplate.Application.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}
