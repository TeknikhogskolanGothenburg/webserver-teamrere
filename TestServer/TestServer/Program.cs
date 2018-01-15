using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestServer
{
    class Program
    {
        private static string pathEnding;

        static void Main(string[] prefixes)
        {
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
            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
            listener.Start();

            Console.WriteLine("Listening...");
            var counter = 1;
            
            while (true)
            {
                counter++;
                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
             
                string url = request.RawUrl;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string path = @"Content\";
            
                try
                {
                    switch (path)
                    {
                        case ".html":
                            Console.WriteLine("html file");
                            break;
                        case ".jpg":
                            Console.WriteLine("jpg file");
                            break;
                        case ".gif":
                            Console.WriteLine(".gif file");
                            break;
                        case ".pdf":
                            Console.WriteLine(".pdf file");
                            break;
                        case ".js":
                            Console.WriteLine(".js file");
                            break;
                        case ".css":
                            Console.WriteLine(".css file");
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

                var cookie = request.Cookies.Count;
                
                if (cookie == 0)
                {
                    var counterCookie = new Cookie("counter", counter.ToString())
                    {
                        Expires = DateTime.Now.AddYears(1)
                    };
                    response.Cookies.Add(counterCookie);
                  
                }


                request.Cookies["counter"].Value = counter.ToString();


                response.AppendCookie(request.Cookies["counter"]);


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
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }



            }

            listener.Stop();
        }
    }
}
