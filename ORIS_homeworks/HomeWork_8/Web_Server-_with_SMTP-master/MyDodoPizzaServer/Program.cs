using MyHttpServer;

internal class Program
{
    private static bool flagIsRunning = true;

    static async Task Main(string[] args)
    {
        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            flagIsRunning = false;
        };

        try
        {
            using (var server = new HttpServer())
            {
                server.Start();

                while (Console.ReadLine() != "stop" && flagIsRunning) { }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Во время работы сервера произошла непредвиденная ошибкаю {0}", ex.Message));
        }
    }
}