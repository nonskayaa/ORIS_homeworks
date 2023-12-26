using HtmlAgilityPack;
using MyDodoPizzaServer.Configuration;
using MyDodoPizzaServer.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace MyHttpServer
{
    public class HttpServer : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private HttpListener httpListener;
        private Appsettings config;
        private string mainUrl;

        public HttpServer()
        {
            config = Configuration.GetConfiguration();
            httpListener = new HttpListener();
            cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine($"{config.Address}:{config.Port}/");
            httpListener.Prefixes.Add($"{config.Address}:{config.Port}/");
            mainUrl = config.Address + config.Port+"//";
        }

        public async void Start()
        {

            httpListener.Start();
            Console.WriteLine($"Сервер запущен. Адрес сервера {config.Address}:{config.Port}");

            await Task.Run(() => { Listener(); }, cancellationTokenSource.Token);

        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            httpListener.Stop();

            Console.WriteLine("Сервер завершил свою работу");
        }

        private async Task Listener()
        {
            const string indexHtml = "/index.html";

            try
            {
                while (true)
                {
                    var context = await httpListener.GetContextAsync();

                    var request = context.Request;

                    var absltPath = request.Url.AbsolutePath;


                    var filePath = config.StaticFilesPath;
                    try
                    {
                        var response = context.Response;
                        var stream = new StreamReader(request.InputStream);
                        using Stream output = response.OutputStream;

                        if (request.HttpMethod.Equals("Post", StringComparison.OrdinalIgnoreCase) && absltPath == "/send-email")
                        {
                            string str = await stream.ReadToEndAsync();
                            string[] formData = str.Split("\n");

                            var emailSender = new EmailSenderService(formData, config);
                            await emailSender.SendEmailAsync();
                            response.Redirect("/");

                        }

                        else if (request.HttpMethod.Equals("Get", StringComparison.OrdinalIgnoreCase))
                        {
                       

                            if (absltPath.ToString() == "/")
                            {

                                if (File.Exists(filePath + indexHtml))
                                {
                                    byte[] buffer = File.ReadAllBytes(filePath + indexHtml);
                                    response.ContentLength64 = buffer.Length;

                                    await output.WriteAsync(buffer);
                                    await output.FlushAsync();

                                    Console.WriteLine($"Отправляем {filePath + indexHtml}");
                                }
                                else
                                {
                                    Console.WriteLine($"Ошибка, не удалось загрузить страницу по пути {filePath + indexHtml}");
                                }
                            }
                            else if (absltPath.ToString().Contains("static"))
                            {

                                var staticPath = filePath + absltPath;
                                var staticHandler = new StaticFilesHandler(staticPath, context, request);
                                await staticHandler.ShowStaticFilesAsync();
                            }
                            else
                            {
                                string responseText =
                                    @"<!DOCTYPE html>
                                    <html>
                                        <head>
                                            <meta charset='utf8'>
                                            <title>ОШИБКА 404</title>
                                        </head>
                                        <body>
                                            <h1>ОШИБКА 404</h1>
                                            <h4>Страница не найдена</h4>
                                        </body>
                                    </html>";
                                Console.WriteLine($"Страница по адресу {request.RawUrl} не найдена");
                                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                                response.ContentLength64 = buffer.Length;

                                await output.WriteAsync(buffer);
                                await output.FlushAsync();
                            }
                        }

                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine(String.Format("Файл {0} не найден", filePath));
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Во время работы сервера произошла непредвиденная ошибка: {0}", ex.Message));
                Stop();
            }

        }

        public void Dispose()
        {
            Stop();
        }

    }
}
