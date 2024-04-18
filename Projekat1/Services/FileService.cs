using System;
using System.Collections.Generic;
using System.IO;
using FileInfo = Projekat1.Models.FileInfo;


namespace Projekat1.Services
{
    public class FileService
    {
        public  static void getFiles(Dictionary<string,List<FileInfo>>dictionary,string path="./")
        {
            var directoryInfo = new DirectoryInfo(path);
            foreach (var file in directoryInfo.GetFiles())
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

            foreach (var dir in directoryInfo.GetDirectories())
            {
                getFiles(dictionary,dir.Name);
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