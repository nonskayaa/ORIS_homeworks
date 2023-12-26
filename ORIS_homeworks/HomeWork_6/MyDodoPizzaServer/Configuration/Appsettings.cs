using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDodoPizzaServer.Configuration
{
    public class Appsettings
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public string StaticFilesPath { get; set; }
        public string EmailSender { get; set; }
        public string Password { get; set; }
        public string EmailTo { get; set; }
        public string SmtpServerHost { get; set; }
        public int SmtpServerPort { get; set; }
    }
}
