using FullTemplate.Application.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace FullTemplate.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        if (result.Succeeded)
            return Result.Success();

        var kind = result.Errors.Any(e => MapKind(e.Code) == ErrorKind.Conflict)
            ? ErrorKind.Conflict
            : ErrorKind.Validation;

        return Result.Failure(kind, result.Errors.Select(e => e.Description));
    }

public static Result<T> ToApplicationResult<T>(this SignInResult result) =>
    result.IsLockedOut  ? Result<T>.Failure(ErrorKind.Forbidden, "Account is locked. Try again later.") :
    result.IsNotAllowed ? Result<T>.Failure(ErrorKind.Forbidden, "Email is not confirmed.") :
                          Result<T>.Failure(ErrorKind.Unauthorized, "Invalid credentials.");
    private static ErrorKind MapKind(string code) => code switch
    {
        nameof(IdentityErrorDescriber.DuplicateEmail) or
        nameof(IdentityErrorDescriber.DuplicateUserName) or
        nameof(IdentityErrorDescriber.ConcurrencyFailure) or
        nameof(IdentityErrorDescriber.UserAlreadyInRole) or
        nameof(IdentityErrorDescriber.UserAlreadyHasPassword) => ErrorKind.Conflict,

        _ => ErrorKind.Validation
    };
}