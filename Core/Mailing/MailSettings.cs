using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mailing;

public class MailSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string FromFullName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public MailSettings()
    {

    }

    public MailSettings(string host, int port, string fromFullName, string userName, string password)
    {
        Host = host;
        Port = port;
        FromFullName = fromFullName;
        UserName = userName;
        Password = password;
    }
}
