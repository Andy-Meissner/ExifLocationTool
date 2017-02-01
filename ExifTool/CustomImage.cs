using System;
using System.Drawing;

namespace ExifTool
{
    public class CustomImage
    {
        private Image _image;
        private string _path;
        private string _bufferpath;

        public CustomImage(string path)
        {
            _path = path;
            _image = Image.FromFile(_path);
            _bufferpath = System.IO.Path.GetDirectoryName(_path) + "\\new_" + System.IO.Path.GetFileName(_path);
        }

        public Image Image
        {
            get { return _image; }
        }
        
        public string Path
        {
            get { return _path; }
        }

        public string Bufferpath
        {
            get { return _bufferpath; }
            set { _bufferpath = value; }
        }

    }
}
