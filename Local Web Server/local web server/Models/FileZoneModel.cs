﻿using System.Collections.Generic;
namespace Local_Web_Server.Models
{
    public class FileZone
    {
        public List<Item> Files { get; set; }
        public List<System.IO.DirectoryInfo> Folders { get; set; }
        public string CurrentDirectory { get; set; }

    }
    public struct Item
    {
        public string Title { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
    }
}