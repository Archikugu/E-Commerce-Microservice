using System.ComponentModel.DataAnnotations;

namespace MultiShop.WebUI.Models;

public class MailRequest
{
    [Required, EmailAddress]
    public string To { get; set; }

    [EmailAddress]
    public string? From { get; set; }

    [Required, StringLength(200)]
    public string Subject { get; set; }

    [Required]
    public string Body { get; set; }

    public bool IsHtml { get; set; }
}


