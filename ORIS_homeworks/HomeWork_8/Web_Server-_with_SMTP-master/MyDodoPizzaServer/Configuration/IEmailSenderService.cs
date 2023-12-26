using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDodoPizzaServer.Configuration
{
    public interface IEmailSenderService
    {
        public string EmailSender { get; set; }
        public string Password { get; set; }
        public string EmailTo { get; set; }
        public string SmtpServerHost { get; set; }
        public int SmtpServerPort { get; set; }
        public string[] FormData { get; set; }

        public async Task SendEmailAsync()
        {
        
        }

    }
}
