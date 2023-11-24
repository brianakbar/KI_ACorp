namespace ACorp.Application;

using ACorp.Models;
using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    const string name = "A Corp";
    const string email = "acorpcorporation.co@gmail.com";
    const string password = "tntl xjex qnky bghw";

    public static void SendEmail(User receiver, string subject, string body)
    {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(name, email));
        mimeMessage.To.Add(new MailboxAddress(receiver.Fullname, receiver.Email));

        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, false);

        smtp.Authenticate(email, password);

        smtp.Send(mimeMessage);
        smtp.Disconnect(true);
    }
}