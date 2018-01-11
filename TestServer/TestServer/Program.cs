﻿using System;
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

        static void Main()
        {
            string[] prefix = { "http://localhost:8080/" };

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefix == null || prefix.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefix)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();

            Cookie TestCookie = new Cookie();
            int CookieCounter = 0;

            Console.WriteLine("Listening...");
            while (true)
            {
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

                    //Cookie Play
                    Console.WriteLine("Ghetto Counter:" + CookieCounter);
                    TestCookie.Value = CookieCounter.ToString();
                    CookieCounter++;
                    response.Cookies.Add(TestCookie);

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
