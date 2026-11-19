namespace FullTemplate.Infrastructure.Identity.Options;

public class AuthLinkOptions
{
    public const string SectionName = "AuthLinks";

    /// <summary>e.g. https://localhost:5001/api/Auth/confirmEmail</summary>
    public string ConfirmEmailUrl { get; set; } = default!;

    /// <summary>e.g. https://myfrontend.com/reset-password</summary>
    public string ResetPasswordUrl { get; set; } = default!;
}
