using System.Net;

namespace Projekat1.LogDir
{
    public class Log
    {
        public HttpStatusCode statusCode { get;  }
        public long contentLength { get;  }
        public string contentType { get;  }
        public byte[] content { get; }

        public Log(HttpStatusCode statusCode, long contentLength, string contentType, byte[] content)
        {
            this.statusCode = statusCode;
            this.contentLength = contentLength;
            this.contentType = contentType;
            this.content = content;
        }
    }
}