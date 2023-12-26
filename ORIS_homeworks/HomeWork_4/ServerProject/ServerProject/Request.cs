using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProject
{
    public class Request
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }
        private Request(string type, string url, string host)
        {
            Type = type;
            Url = url;
            Host = host;
        }
        public static Request GetRequest(string request)
        {
            if (string.IsNullOrEmpty(request))
                return null;
            string[] requestTokens = request.Split(' ');
            string type = requestTokens[0];
            string url = requestTokens[1];
            string host = requestTokens[4];
            return new Request(type, url, host);
        }
    }
}
