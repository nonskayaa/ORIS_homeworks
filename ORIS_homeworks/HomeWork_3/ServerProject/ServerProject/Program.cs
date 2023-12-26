namespace ServerProject
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port ");
            HTTPServer server = new HTTPServer(8080);
            server.Start();
        }
    }
}