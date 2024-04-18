﻿using System;
using System.Collections.Generic;
using Projekat1.Models;
using Projekat1.Server;
using Projekat1.Services;

namespace Projekat1
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            Dictionary<string, List<FileInfo>> dictionary = new Dictionary<string, List<FileInfo>>();
            FileService.getFiles(dictionary);
            HttpServer server = new HttpServer("http://localhost:5050/", dictionary);
            server.start();
        }
    }
}