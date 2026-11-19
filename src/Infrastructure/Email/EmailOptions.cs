namespace FullTemplate.Infrastructure.Identity;

public class EmailOptions
{
    public const string SectionName = "Email";

    public string Host { get; set; } = default!;
    public int Port { get; set; } = 587;

    public string? User { get; set; }

    public string? Password { get; set; }

    public string FromAddress { get; set; } = default!;
    public string FromName { get; set; } = "FullTemplate";
}