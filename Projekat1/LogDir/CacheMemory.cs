using System;
using System.Collections.Generic;
using System.Threading;
using Projekat1.LogDir;

namespace Projekat1.Cache
{
    public class CacheMemory
    {
        private Dictionary<string, Log> dictionaryLog = new Dictionary<string, Log>();
        private static readonly object locker = new object();
        public (bool, Log) getLog(string url)
        {
            bool ima=false;
            Log l = null;
            lock (locker)
            {
                try
                {
                    ima = dictionaryLog.TryGetValue(url, out l);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return (ima, l);
        }

        public void writeResponse(string url, Log l)
        {
            lock (locker)
            {
                try
                {
                    dictionaryLog.Add(url, l);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Timer t = new Timer(removeResponse, url, TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(Timeout.InfiniteTimeSpan.Seconds));
            
        }

        public void removeResponse(object url)
        {
            string key = (string)url;
            Console.WriteLine(key);
            Console.WriteLine("Tring to remove hash");
            lock (locker)
            {
                dictionaryLog.Remove(key);
            }
        }
    }
}