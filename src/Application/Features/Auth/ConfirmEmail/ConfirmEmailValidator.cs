namespace FullTemplate.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Confirmation code is required.");
    }
}
