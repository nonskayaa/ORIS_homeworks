using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyDodoPizzaServer.Configuration
{
    public class StaticFilesHandler
    {
        public string Path { get; set; }
        public HttpListenerRequest Request { get; set; }
        public HttpListenerContext httpListenerContext { get; set; }
        public StaticFilesHandler(string path, HttpListenerContext httpListenerContext, HttpListenerRequest request)

        {
            this.Path = path;
            this.httpListenerContext = httpListenerContext;
            this.Request = request;
        }

        public async Task ShowStaticFilesAsync()
        {
            var response = httpListenerContext.Response;
            var stream = new StreamReader(Request.InputStream);
            using Stream output = response.OutputStream;
            if (File.Exists(Path))
            {
                byte[] buffer = File.ReadAllBytes(Path);
                response.ContentLength64 = buffer.Length;

                await output.WriteAsync(buffer);
                await output.FlushAsync();
                Console.WriteLine($"Отправляем {Path}");
            }
            else
            {
                Console.WriteLine($"Ошибка, не удалось загрузить статический файл по пути {Path}");
            }
        }
    }
}
