﻿using System;
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
        {  while (true)
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
                foreach (string s in prefixes)
                {
                    listener.Prefixes.Add(s);
                }
                listener.Start();
                Console.WriteLine("Listening...");
                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string path = "Content";
                string [] responseString = Directory.GetFiles(path);
                byte[] buffer = new byte[responseString.Length];
                for (int i = 0; i < responseString.Length; i++)
                {
                    buffer = Encoding.UTF8.GetBytes(responseString[i]);
                }
                // Get a response stream and write the response to it.
                response.ContentLength64 = responseString.Length;
                System.IO.Stream output = response.OutputStream;
                
                    output.Write(buffer, 0, buffer.Length);
                
                // You must close the output stream.
                output.Close();
                listener.Stop();
            }
        }
    }
}
