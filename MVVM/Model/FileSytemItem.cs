using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppTotalCommander.MVVM.Model
{
    public class FileSystemItem
    {
        public string Name { get; }
        public string Type { get; }
        public DateTime Date { get; }
        public long Size { get; }
        public string RootPath { get; set; }
        public FileSystemInfo Info { get; set; }

        public FileSystemItem(FileSystemInfo info)
        {
            Info = info;
            Name = info.Name;
            Type = info is DirectoryInfo ? "Folder" : "File";
            Date = info.LastWriteTime;
            Size = (info is FileInfo fileInfo) ? fileInfo.Length : 0;
            RootPath = info.FullName;
        }
    }
}
