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
        
    }

    public Task SendMailAsync(Mail mail)
    {
        throw new NotImplementedException();
    }
}
