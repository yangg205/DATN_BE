using Microsoft.Extensions.Options;
using MimeKit;

namespace DATNBACK_END.Helpers;
public class EmailSetting
{
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? From { get; set; }
}
public interface IEmailService
{
    void SendOTPEmail(string toEmail, string subject, string otp);
}
public class EmailService : IEmailService
{
    private readonly EmailSetting _emailSetting;
    public EmailService(IOptions<EmailSetting> emailSetting)
    {
        _emailSetting = emailSetting.Value;
    }
     public void SendOTPEmail(string toEmail, string subject, string otp)
    {
        try
        {
            var message = new MimeMessage();
            var bodybuilder = new BodyBuilder();
            message.From.Add(new MailboxAddress(_emailSetting.Name, _emailSetting.From));
            message.To.Add(new MailboxAddress("yangkaka", toEmail));
            message.Subject = subject;

            using (var reader = System.IO.File.OpenText("Helpers/OTPTemplate.html")){
                bodybuilder.HtmlBody = reader.ReadToEnd();
                bodybuilder.HtmlBody = bodybuilder.HtmlBody.Replace("[OTP_CODE]", otp);
            }

            message.Body = bodybuilder.ToMessageBody();
            // message.Body = new TextPart(TextFormat.Html)
            // {
            //     Text = body
            // };
        
            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect(_emailSetting.Host, 
                    int.Parse(_emailSetting.Port), MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(_emailSetting.From, _emailSetting.Password);
            client.Send(message);
            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while sending email." + ex.Message);
        }
    }
}
