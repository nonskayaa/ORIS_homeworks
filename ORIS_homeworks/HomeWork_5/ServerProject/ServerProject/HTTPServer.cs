using ServerProject.configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerProject
{
    public class HTTPServer
    {


        public const string msgDirection = "\\root\\msg";
        public const string webFiles = "\\webFiles";
        public const string httpVersion = "HTTP/1.1";
        public const string nameOfServer = "MyServer HTTP Server v0.1";
        private bool running = false;
        private TcpListener tcpListener;
        private HttpListener httpListener;

        public HTTPServer(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            httpListener = new HttpListener();
        }

       
            public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        private void Run()
        {
            running = true;
            tcpListener.Start();
            while (running)
            {
                Console.WriteLine("Connection");
                TcpClient client = tcpListener.AcceptTcpClient();
                //Принимает ожидающий запрос на подключение.
                Console.WriteLine("Connection is succesfull");
                HandleClient(client);
                client.Close();
            }
            running = false;
            tcpListener.Stop();
        }

        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());
            string msg = "";
            while (reader.Peek() != -1) //пока есть пользователи 
            {
                msg += reader.ReadLine() + "\n"; 
            }
            Debug.WriteLine("Request: \n" + msg);

            Request req = Request.GetRequest(msg);
            Response resp = Response.From(req);
            resp.Post(client.GetStream());
        }

    }
}
