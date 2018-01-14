using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public sealed class HttpCookie { }

    class Program
    {
       
        static void Main(string[] prefixes)
        {
            int cookieName = 0;
            
            Dictionary<string, int> cookieDict = new Dictionary<string, int>();

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
            HttpListenerContext context = listener.GetContext();
            HttpListenerContext cookie = listener.GetContext();
            HttpListenerResponse response = context.Response;
            while (true)
            {

                // Note: The GetContext method blocks while waiting for a request. 
                if (response.Cookies.Count == 0)
                {
                    cookieName++;
                    response.Cookies.Add(new Cookie(cookieName.ToString(), "0"));
                    cookieDict.Add("Cookie: " + cookieName, 0);
                    context = listener.GetContext();

                }

                else
                {
                    context = listener.GetContext();
                }

                HttpListenerRequest request = context.Request;
                cookieDict["Cookie: " + cookieName]++;

                string url = request.RawUrl;

                // Obtain a response object.
                response = context.Response;
                if (request.RawUrl.StartsWith("/counter")) {
                    string responseString = "Cookie: " + cookieName +" "+ cookieDict["Cookie: " + cookieName].ToString();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    continue;
                }

                //response.Headers.Add(HttpResponseHeader.Expires, DateTime.Now.AddYears(1).ToString());
                // Construct a response.
                string path = @"Content\";
                try
                {
                    if (url.Substring(url.IndexOf('.'), url.Length - url.IndexOf('.')) == ".jpeg"
                        || url.Substring(url.IndexOf('.'), url.Length - url.IndexOf('.')) == ".jpg")
                    {
                        response.ContentType = "image/jpeg";
                    }
                }
                catch { }



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
                    // status 404
                }

            }

            listener.Stop();
        }
    }
}