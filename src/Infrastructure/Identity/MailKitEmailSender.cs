using System.Text;
using Heven.Api.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Heven.Api.Infrastructure.Identity;

public sealed class MailKitEmailSender : IEmailSender<ApplicationUser>
{
    private readonly EmailSettings _settings;
    private readonly ILogger<MailKitEmailSender> _logger;

    public MailKitEmailSender(IOptions<EmailSettings> options, ILogger<MailKitEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var link = NormalizeQuerySeparators(confirmationLink);
        var subject = "Confirm your Heven email";
        var text = new StringBuilder()
            .AppendLine("Open the link below to confirm your account:")
            .AppendLine()
            .AppendLine(link)
            .ToString();

        var html = new StringBuilder()
            .Append("<p>Open the link below to confirm your account:</p>")
            .Append("<p><a href=\"").Append(HtmlEncode(link)).Append("\">Confirm email</a></p>")
            .Append("<p>Or copy this URL:</p>")
            .Append("<p style=\"word-break:break-all\">").Append(HtmlEncode(link)).Append("</p>")
            .ToString();

        return SendAsync(email, subject, text, html);
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var link = NormalizeQuerySeparators(resetLink);
        var subject = "Reset your Heven password";
        var text = $"Reset your password using this link:\n\n{link}";
        var html = $"<p><a href=\"{HtmlEncode(link)}\">Reset password</a></p><p style=\"word-break:break-all\">{HtmlEncode(link)}</p>";
        return SendAsync(email, subject, text, html);
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var subject = "Your Heven password reset code";
        var text = $"Your password reset code: {resetCode}";
        var html = $"<p>Your password reset code: <strong>{HtmlEncode(resetCode)}</strong></p>";
        return SendAsync(email, subject, text, html);
    }

    private async Task SendAsync(string to, string subject, string textBody, string? htmlBody)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Email:Enabled=false — skipping send to {To}, subject {Subject}", to, subject);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.FromAddress)
            || string.IsNullOrWhiteSpace(_settings.UserName)
            || string.IsNullOrWhiteSpace(_settings.Password))
        {
            _logger.LogWarning("Email settings incomplete (FromAddress/UserName/Password) — skipping send to {To}", to);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromDisplayName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var body = new BodyBuilder { TextBody = textBody };
        if (htmlBody is not null)
            body.HtmlBody = htmlBody;
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();
        var secure = _settings.SmtpPort == 465
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, secure).ConfigureAwait(false);
        await client.AuthenticateAsync(_settings.UserName, _settings.Password.Trim().Replace(" ", string.Empty)).ConfigureAwait(false);
        await client.SendAsync(message).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
    }

    private static string HtmlEncode(string value) =>
        System.Net.WebUtility.HtmlEncode(value) ?? string.Empty;

    private static string NormalizeQuerySeparators(string url) =>
        url.Replace("&amp;", "&", StringComparison.Ordinal);
}
