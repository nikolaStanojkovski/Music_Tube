using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain
{
    public class EmailSettings
    {
        public string SMTPServer { get; set; }

        public string SMTPUsername { get; set; }

        public string SMTPPassword { get; set; }

        public int SMTPServerPort { get; set; }

        public bool EnableSSL { get; set; }

        public string EmailDisplayName { get; set; }

        public string SenderName { get; set; }

        public EmailSettings(string _SMTPServer, string _SMTPUsername, string _SMTPPassword, int _SMTPServerPort)
        {
            SMTPServer = _SMTPServer;
            SMTPUsername = _SMTPUsername;
            SMTPPassword = _SMTPPassword;
            SMTPServerPort = _SMTPServerPort;
        }

        public EmailSettings()
        {

        }
    }
}
