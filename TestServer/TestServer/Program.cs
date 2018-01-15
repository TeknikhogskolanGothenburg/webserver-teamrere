using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {

        static void Main(string[] prefixes)
        {
            int counter = 0;

            Dictionary<string, int> cookie = new Dictionary<string, int>();

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();

            Console.WriteLine("Listening...");
            while (true)
            {

                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                if (context.Response.Cookies.Count == 0)
                {
                    counter++;
                    cookie.Add("Cookie: " + counter, 0);
                }

                HttpListenerRequest request = context.Request;
              
                string url = request.RawUrl;

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                if (request.RawUrl.StartsWith("/counter"))
                {
                    string responseString = "Request: " + counter.ToString();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    continue;
                }

                response.Headers.Add(HttpResponseHeader.Expires, DateTime.Now.AddYears(1).ToString());
                // Construct a response.
                string path = @"Content\";
                try
                {

                    switch (path)
                    {
                        case ".html":
                            Console.WriteLine("Response was content type text/html");
                            response.AddHeader("Content-Type", "text/html");
                            break;
                        case ".jpg":
                            Console.WriteLine("Response was content type image/jpeg");
                            response.AddHeader("Content-Type", "image/jpeg");
                            break;
                        case ".gif":
                            Console.WriteLine("Response was content type image/gif");
                            response.AddHeader("Content-Type", "image/gif");
                            break;
                        case ".pdf":
                            Console.WriteLine("Response was content type application/pdf");
                            response.AddHeader("Content-Type", "application/pdf");
                            break;
                        case ".js":
                            Console.WriteLine("Response was content type application/javascript");
                            response.AddHeader("Content-Type", "application/javascript");
                            break;
                        case ".css":
                            Console.WriteLine("Response was content type css");
                            response.AddHeader("Content-Type", "text/css");
                            break;

                        default:
                            Console.WriteLine("File type inncorect");
                            break;
                    }
                }
                catch
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                if (File.Exists((path + url)))
                {
                    byte[] buffer = File.ReadAllBytes(path + url);

                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;

                    output.Write(buffer, 0, buffer.Length);

                    // You must close the output stream.
                    output.Close();
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            listener.Stop();
        }
    }
}

