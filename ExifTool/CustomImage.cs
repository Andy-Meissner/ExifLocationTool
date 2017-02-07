﻿using System;
using System.Drawing;

namespace ExifTool
{
    public class CustomImage
    {
        public CustomImage(string path)
        {
            Path = path;
            Image = Image.FromFile(Path);
            Bufferpath = System.IO.Path.GetDirectoryName(Path) + "\\new_" + System.IO.Path.GetFileName(Path);
        }

        public Image Image { get; }

        public string Path { get; }

        public string Bufferpath { get; }
    }
}
