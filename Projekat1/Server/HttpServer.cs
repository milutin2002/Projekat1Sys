using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Projekat1.LogDir;
using Projekat1.Services;
using FileInfo = Projekat1.Models.FileInfo;

namespace Projekat1.Server
{
    public class HttpServer
    {
        private HttpListener _listener;
        private Dictionary<string, List<FileInfo>> _dictionary;
        private Dictionary<string, Log> dictionaryLog = new Dictionary<string, Log>();
        private byte[][] responses;
        public HttpServer(string prefix,Dictionary<string,List<FileInfo>>_dictionary)
        {
            this._listener = new HttpListener();
            this._listener.Prefixes.Add(prefix);
            this._dictionary = _dictionary;
            this.responses = new[] { Encoding.UTF8.GetBytes("This isn't file"),Encoding.UTF8.GetBytes("The file is not found"),Encoding.UTF8.GetBytes("The file is found but extension is bad") };
        }

        public void start()
        {
            _listener.Start();
            while (true)
            {
                ThreadPool.QueueUserWorkItem(handleRequest, _listener.GetContext());
            }
        }

        private void handleRequest(object data)
        {
            HttpListenerContext listenerContext = (HttpListenerContext)data;
            var request = listenerContext.Request;
            var respone = listenerContext.Response;
            var url = request.Url.AbsolutePath;
            var cacheLog = LogUtils.getLog(dictionaryLog, url);
            if (cacheLog.Item1)
            {
                sendResponseWithoutSave(respone,cacheLog.Item2.statusCode,cacheLog.Item2.contentLength,cacheLog.Item2.content,cacheLog.Item2.contentType);
                return;
            }
            var urlPath = request.Url.AbsolutePath.Substring(1);
            var ext = FileService.GetFileExtension(urlPath);
            if (ext == "")
            {
                sendResponse(respone,HttpStatusCode.BadRequest,responses[0].Length,responses[0],"text/plain",url);
            }
            else
            {
                var fileName = urlPath.Substring(0, urlPath.Length - ext.Length);
                try
                {
                    var fileInfo = FileService.findFile(fileName, ext, _dictionary);
                    if (fileInfo == null)
                    {
                        sendResponse(respone,HttpStatusCode.NotFound,responses[1].Length,responses[1],"text/plain",url);
                    }
                    else
                    {
                        var fileBytes = File.ReadAllBytes(fileInfo.Path +"/"+ fileInfo.Name+fileInfo.Extension);
                        sendResponse(respone,HttpStatusCode.OK,fileBytes.Length,fileBytes,"image/jpg",url);
                    }
                }
                catch (BadExtensionExcpetion e)
                {
                    sendResponse(respone,HttpStatusCode.BadRequest,responses[2].Length,responses[2],"text/plain",url);
                }
            }
        }

        
        private void sendResponseWithoutSave(HttpListenerResponse response, HttpStatusCode code, long length, byte[] bytes,string contentType)
        {
            response.StatusCode = (int)code;
            response.ContentLength64 = length;
            response.ContentType = contentType;
            using (Stream output = response.OutputStream)
            {
                output.Write(bytes,0,bytes.Length);
            }
            response.Close();
        }

        private void sendResponse(HttpListenerResponse response, HttpStatusCode code, long length, byte[] bytes,string contentType,string url)
        {
            sendResponseWithoutSave(response,code,length,bytes,contentType);
            LogUtils.writeResponse(dictionaryLog,url,new Log(code,length,contentType,bytes));
        }
    }
}