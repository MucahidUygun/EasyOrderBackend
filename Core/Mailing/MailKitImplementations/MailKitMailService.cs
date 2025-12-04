using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mailing.MailKitImplementations;

public class MailKitMailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailKitMailService(MailSettings configuration)
    {
        _mailSettings = configuration;
    }

    public void SendMail(Mail mail)
    {
        if (mail.ToList != null && mail.ToList.Count >= 1)
        {
            emailPrepare(mail, out MimeMessage email, out SmtpClient smtp);
            smtp.Send(email);
            smtp.Disconnect(quit: true);
            email.Dispose();
            smtp.Dispose();
        }
    }

    public async Task SendMailAsync(Mail mail)
    {
        if (mail.ToList != null && mail.ToList.Count >= 1)
        {
            emailPrepare(mail, out MimeMessage email, out SmtpClient smtp);
            await smtp.SendAsync(email);
            smtp.Disconnect(quit: true);
            email.Dispose();
            smtp.Dispose();
        }
    }
    private void emailPrepare(Mail mail, out MimeMessage email, out SmtpClient smtp)
    {
        email = new MimeMessage();
        email.From.Add(new MailboxAddress(_mailSettings.SenderFullName, _mailSettings.SenderEmail));
        email.To.AddRange(mail.ToList);
        if (mail.CcList != null && mail.CcList.Any())
        {
            email.Cc.AddRange(mail.CcList);
        }

        if (mail.BccList != null && mail.BccList.Any())
        {
            email.Bcc.AddRange(mail.BccList);
        }

        email.Subject = mail.Subject;
        if (mail.UnsubscribeLink != null)
        {
            email.Headers.Add("List-Unsubscribe", "<" + mail.UnsubscribeLink + ">");
        }

        BodyBuilder bodyBuilder = new BodyBuilder
        {
            TextBody = mail.TextBody,
            HtmlBody = mail.HtmlBody
        };
        if (mail.Attachments != null)
        {
            foreach (MimeEntity attachment in mail.Attachments)
            {
                if (attachment != null)
                {
                    bodyBuilder.Attachments.Add(attachment);
                }
            }
        }

        email.Body = bodyBuilder.ToMessageBody();
        email.Prepare(EncodingConstraint.SevenBit);
        if (_mailSettings.DkimPrivateKey != null && _mailSettings.DkimSelector != null && _mailSettings.DomainName != null)
        {
            DkimSigner dkimSigner = new DkimSigner(readPrivateKeyFromPemEncodedString(), _mailSettings.DomainName, _mailSettings.DkimSelector)
            {
                HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                AgentOrUserIdentifier = "@" + _mailSettings.DomainName,
                QueryMethod = "dns/txt"
            };
            HeaderId[] headers = new HeaderId[3]
            {
                HeaderId.From,
                HeaderId.Subject,
                HeaderId.To
            };
            dkimSigner.Sign(email, headers);
        }

        smtp = new SmtpClient();
        smtp.Connect(_mailSettings.Server, _mailSettings.Port);
        if (_mailSettings.AuthenticationRequired)
        {
            smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
        }
    }
    private AsymmetricKeyParameter readPrivateKeyFromPemEncodedString()
    {
        using StringReader reader = new StringReader("-----BEGIN RSA PRIVATE KEY-----\n" + _mailSettings.DkimPrivateKey + "\n-----END RSA PRIVATE KEY-----");
        return ((AsymmetricCipherKeyPair)new PemReader(reader).ReadObject()).Private;
    }
}
