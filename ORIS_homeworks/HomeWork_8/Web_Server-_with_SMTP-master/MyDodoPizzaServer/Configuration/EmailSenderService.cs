using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyDodoPizzaServer.Configuration
{
    public class EmailSenderService : IEmailSenderService
    {
        public string EmailSender { get;  set; }
        public string Password { get;  set; }
        public string EmailTo { get;  set; }
        public string SmtpServerHost { get;  set; }
        public int SmtpServerPort { get;  set; }
        public string[] FormData { get;  set; }
        public EmailSenderService(string[] formData, Appsettings config)
        {
            this.FormData = formData;
            this.EmailSender = config.EmailSender;
            this.Password = config.Password;
            this.EmailTo = config.EmailTo;
            this.SmtpServerHost = config.SmtpServerHost;
            this.SmtpServerPort = config.SmtpServerPort;
        }
        public async Task SendEmailAsync()
        {
            MailAddress from = new MailAddress(EmailSender, "Leisan");
            MailAddress to = new MailAddress(EmailTo);
            MailMessage m = new MailMessage(from, to);

            m.Subject = "Домашняя Работа ОРИС";
            m.Body = $"{FormData[3]}\n{FormData[7]}\n{FormData[11]}\n{FormData[15]}\n{FormData[19]}\n{FormData[23]}\n{FormData[27]}";
            m.IsBodyHtml = false;

            Console.WriteLine($"ОТПРАВЛЯЕМ СООБЩЕНИЕ ОТ  : {EmailSender} КОМУ : {EmailTo} ");
            Console.WriteLine("_________________");
            Console.WriteLine(m.Body);


            SmtpClient smtp = new SmtpClient(SmtpServerHost, SmtpServerPort);
            smtp.Credentials = new NetworkCredential(EmailSender, Password);
            smtp.EnableSsl = true;

            m.Attachments.Add(new Attachment("D:\\ITIS\\ОРИС\\ServerDodoPizza_Hw\\MyDodoPizzaServer.zip"));

            await smtp.SendMailAsync(m);
            Console.WriteLine("Сообщение отправлено");

        }


    }
}
