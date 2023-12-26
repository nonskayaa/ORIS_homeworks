using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServerProject
{
    public class Response
    {
        private string status;
        private string type;
        private Byte[] data = null;
        private Response(string status, string type, Byte[] data)
        {
            this.data = data;
            this.status = status;
            this.type = type;
        }
        public static Response From(Request request)
        {
            if (request == null)
            {
                Console.WriteLine("null!!!");
                return DisplayBadRequest();
            }
            if (request.Type == "GET")
            {

                Console.WriteLine(request.Url);
                if (request.Url.Contains("send-email"))
                {
                    SendEmail(request);
                }
                
                string filePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + HTTPServer.webFiles + request.Url;
                Console.WriteLine("Getting filePath");
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists && fileInfo.Extension.Contains("."))
                {
                    return DisplayPageFrom(fileInfo);
                }
                else
                {

                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath + "\\");
                    if (directoryInfo.Exists)
                    {
                        FileInfo[] filesInfoArray = directoryInfo.GetFiles();
                        foreach (FileInfo file in filesInfoArray)
                        {

                            if (file.Name.Contains("index.html"))
                            {
                                fileInfo = file;
                            }
                            return DisplayPageFrom(fileInfo);
                        }
                    }
                }

            }

            else
            {
                Debug.WriteLine("прошел (");
                DisplayMethodNotAllowed();
            }
            return DisplayPageNotFound();


        }


        private static void SendEmail(Request request)
        {
            DodoPizzaForm dataForm = DodoPizzaForm.GetDataFromDodoPizzaForm(request.Url);

            MailAddress from = new MailAddress("leisannonskaya@yandex.ru", "Leisan");
            //MailAddress to = new MailAddress("minetown2222@gmail.com");
            MailAddress to = new MailAddress("mklim04@mail.ru");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Домашняя Работа ОРИС";
            // текст письма
            m.Body = $"<h1>Добрый день, вот моя домашняя работа :)</h1>\r\n<h3>От {HttpUtility.UrlDecode(dataForm.UserName)} {HttpUtility.UrlDecode(dataForm.UserLastName)}</h3>";
            Console.WriteLine(m.Body);
            //письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("leisannonskaya@yandex.ru", "yccxlqasdrhnkirh");
            //m.Attachments.Add(new Attachment("D://DodoPizza_Server-Project.rar"));
            smtp.EnableSsl = true;
            //m.Attachments.Add(new Attachment("D://Dodo_project.zip"));
            m.Attachments.Add(new Attachment("D://dodoPizza-Server.zip"));
            smtp.Send(m);
        }


        private static Response DisplayBadRequest()
        {
            string filePath = Environment.CurrentDirectory + HTTPServer.msgDirection + "\\400.html";
            FileInfo fileInfo = new FileInfo(filePath);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            byte[] response = new byte[fileStream.Length];
            reader.Read(response, 0, response.Length);
            return new Response("404 Bad Request", "text/html", new byte[0]);
        }
        private static Response DisplayPageFrom(FileInfo fileInfo)
        {
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            byte[] response = new byte[fileStream.Length];
            reader.Read(response, 0, response.Length);
            fileStream.Close();
            return new Response("200 OK", "text/html", response);

        }
        private static Response DisplayPageNotFound()
        {
            string filePath = Environment.CurrentDirectory + HTTPServer.msgDirection + "\\404.html";
            FileInfo fileInfo = new FileInfo(filePath);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            byte[] response = new byte[fileStream.Length];
            reader.Read(response, 0, response.Length);
            fileStream.Close();
            return new Response("404 Page Not Found", "text/html", new byte[0]);
        }
        private static Response DisplayMethodNotAllowed()
        {
            string filePath = Environment.CurrentDirectory + HTTPServer.msgDirection + "\\405.html";
            FileInfo fileInfo = new FileInfo(filePath);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] response = new byte[fileStream.Length];
            reader.Read(response, 0, response.Length);
            fileStream.Close();

            return new Response("405 Method Not Allowed", "text/html", new byte[0]);
        }
        public void Post(NetworkStream stream)
        {

            StreamWriter streamwriter = new StreamWriter(stream);
            streamwriter.WriteLine(string.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\n", HTTPServer.httpVersion, status, HTTPServer.nameOfServer, type, data.Length));
            streamwriter.Flush();
            stream.Write(data, 0, data.Length);
        }

    }
}
