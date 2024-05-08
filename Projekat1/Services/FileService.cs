using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FileInfo = Projekat1.Models.FileInfo;


namespace Projekat1.Services
{
    public class FileService
    {
        private static SpinLock _spinLock = new SpinLock();
        public  static void getFiles(Dictionary<string,List<FileInfo>>dictionary,string path="./")
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            var directoryInfo = new DirectoryInfo(path);
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            //List<ManualResetEvent> events = new List<ManualResetEvent>();
            foreach (var file in files)
            {
                /*if (file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png") || file.Name.EndsWith(".gif"))
                {
                    var resetEvent = new ManualResetEvent(false);
                    events.Add(resetEvent);
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            ProcesFileWithLock(dictionary, file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            resetEvent.Set();
                        }
                    });
                }*/
                ProcesFile(dictionary, file);
            }
            
            /*for (int i = 0; i < events.Count; i++)
            {
                events[i].WaitOne();
            }*/
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);
        }

        private static void ProcesFile(Dictionary<string, List<FileInfo>> dictionary, System.IO.FileInfo file)
        {
            if (file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png") || file.Name.EndsWith(".gif"))
            {
                List<FileInfo> list;
                var fileName = file.Name.Substring(0,file.Name.Length - file.Extension.Length);
                    var ima = dictionary.TryGetValue(fileName, out list);
                    if (!ima)
                    {
                        list = new List<FileInfo>();
                        dictionary.Add(fileName, list);
                    }
                    list.Add(new FileInfo(fileName,file.DirectoryName,file.Extension));   
            }
        }
        private static void ProcesFileWithLock(Dictionary<string, List<FileInfo>> dictionary, System.IO.FileInfo file)
        {
                List<FileInfo> list;
                var fileName = file.Name.Substring(0,file.Name.Length - file.Extension.Length);
                lock (dictionary)
                {
                    var ima = dictionary.TryGetValue(fileName, out list);
                    if (!ima)
                    {
                        list = new List<FileInfo>();
                        dictionary.Add(fileName, list);
                    }

                    list.Add(new FileInfo(fileName, file.DirectoryName, file.Extension));
                }
        }
        
        
        public static FileInfo findFile(string name, string ext,Dictionary<string,List<FileInfo>>dictionary)
        {
            List<FileInfo> list;
            var ima=dictionary.TryGetValue(name, out list);
            if (!ima)
            {
                return null;
            }

            foreach (var file in list)
            {
                if (file.Extension == ext)
                {
                    return file;
                }
            }

            throw new BadExtensionExcpetion();
        }
        public static string GetFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            int lastDotIndex = filePath.LastIndexOf('.');
            if (lastDotIndex == -1 || lastDotIndex == filePath.Length - 1)
                return string.Empty;

            return filePath.Substring(lastDotIndex);
        }
    }
}