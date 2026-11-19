using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FullTemplate.Infrastructure.Identity;

/// <summary>Sends Identity emails through an SMTP server using MailKit.</summary>
internal sealed class EmailSender(
    IOptions<EmailOptions> options,
    ILogger<EmailOptions> logger) : IEmailSender<ApplicationUser>
{
    private readonly EmailOptions _smtp = options.Value;

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        => SendAsync(
            email,
            subject: "Confirm your email",
            html: $"""
                   <p>Hello,</p>
                   <p>Please confirm your account by clicking the link below:</p>
                   <p><a href="{WebUtility.HtmlEncode(confirmationLink)}">Confirm email</a></p>
                   <p>If you did not create an account, you can ignore this email.</p>
                   """,
            text: $"Please confirm your account: {confirmationLink}");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        => SendAsync(
            email,
            subject: "Reset your password",
            html: $"""
                   <p>Hello,</p>
                   <p>You asked to reset your password. Click the link below to choose a new one:</p>
                   <p><a href="{WebUtility.HtmlEncode(resetLink)}">Reset password is</a></p>
                   <p>If you did not ask for this, you can ignore this email.</p>
                   """,
            text: $"Reset your password: {resetLink}");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        => SendAsync(
            email,
            subject: "Your password reset code",
            html: $"""
                   <p>Hello,</p>
                   <p>Your password reset code is: <b>{WebUtility.HtmlEncode(resetCode)}</b></p>
                   <p>If you did not ask for this, you can ignore this email.</p>
                   """,
            text: $"Your password reset code is: {resetCode}");

    private async Task SendAsync(string to, string subject, string html, string text)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = html, TextBody = text }.ToMessageBody();

        try
        {
            using var client = new SmtpClient();

            // Auto picks StartTLS on 587 and SSL on 465
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.Auto);

            if (!string.IsNullOrEmpty(_smtp.User) && !string.IsNullOrEmpty(_smtp.Password))
            {
                await client.AuthenticateAsync(_smtp.User, _smtp.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email '{Subject}' to {Email}", subject, to);
            throw;
        }
    }
}