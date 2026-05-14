namespace Heven.Api.Infrastructure.Configuration;

public sealed class EmailSettings
{
    public const string SectionName = "Email";

    /// <summary>Khi false, không kết nối SMTP (dùng cho test/CI).</summary>
    public bool Enabled { get; set; }

    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;

    /// <summary>Gmail: dùng App Password (16 ký tự, có thể có khoảng trắng khi copy từ Google).</summary>
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FromAddress { get; set; } = string.Empty;
    public string FromDisplayName { get; set; } = "Heven";
}
