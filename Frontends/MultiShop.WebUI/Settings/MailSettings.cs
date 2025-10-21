namespace MultiShop.WebUI.Settings;

public class MailSettings
{
    public string FromName { get; set; }
    public string FromAddress { get; set; }
    public SmtpSettings Smtp { get; set; }
}

public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool UseSsl { get; set; }
    public bool SkipCertificateRevocationCheck { get; set; }
}


