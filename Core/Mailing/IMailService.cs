using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mailing;

public interface IMailService
{
    Task SendMailAsync(Mail mail);
    void SendMail(Mail mail);
}
