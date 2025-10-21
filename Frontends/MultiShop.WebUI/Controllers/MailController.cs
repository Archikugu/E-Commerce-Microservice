using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Models;
using MultiShop.WebUI.Settings;

namespace MultiShop.WebUI.Controllers;

public class MailController : Controller
{
    private readonly MailSettings _mailSettings;

    public MailController(IOptions<MailSettings> mailOptions)
    {
        _mailSettings = mailOptions.Value;
    }

    // GET: /Mail/SendMail
    [HttpGet]
    public IActionResult SendMail()
    {
        return View();
    }

    // POST: /Mail/Send
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Send(MailRequest mailRequest)
    {
        if (!ModelState.IsValid)
        {
            return View("SendMail", mailRequest);
        }

        var message = new MimeMessage();
        var fromName = string.IsNullOrWhiteSpace(_mailSettings.FromName) ? "MultiShop ECommerce" : _mailSettings.FromName;
        // Gmail policy: From should match authenticated account or a verified alias.
        var effectiveFromAddress = _mailSettings.Smtp.Username;
        message.From.Add(new MailboxAddress(fromName, effectiveFromAddress));
        if (!string.IsNullOrWhiteSpace(mailRequest.From))
        {
            // Preserve the user-provided address as Reply-To so recipients can respond to it
            message.ReplyTo.Add(MailboxAddress.Parse(mailRequest.From));
        }
        message.To.Add(MailboxAddress.Parse(mailRequest.To));
        message.Subject = mailRequest.Subject;

        var builder = new BodyBuilder();
        if (mailRequest.IsHtml)
        {
            builder.HtmlBody = mailRequest.Body;
        }
        else
        {
            builder.TextBody = mailRequest.Body;
        }
        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        if (_mailSettings.Smtp.SkipCertificateRevocationCheck)
        {
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtp.CheckCertificateRevocation = false;
        }

        var socketOption = _mailSettings.Smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
        smtp.Connect(_mailSettings.Smtp.Host, _mailSettings.Smtp.Port, socketOption);
        // Force basic auth with Gmail (avoid XOAUTH2 when using app-password)
        smtp.AuthenticationMechanisms.Remove("XOAUTH2");
        smtp.Authenticate(_mailSettings.Smtp.Username, _mailSettings.Smtp.Password);
        smtp.Send(message);
        smtp.Disconnect(true);

        TempData["MailResult"] = "Your message has been sent successfully";
        ModelState.Clear();
        return RedirectToAction("SendMail");
    }
}


