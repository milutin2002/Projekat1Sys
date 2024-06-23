using System;
using System.Collections.Generic;
using System.Threading;
using Projekat1.Models;
using Projekat1.Server;
using Projekat1.Services;

namespace Projekat1
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            //ThreadPool.SetMaxThreads(10,100);
            HttpServer server = new HttpServer("http://localhost:5050/");
            server.start();
        }
    }
}