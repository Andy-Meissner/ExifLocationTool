using System;
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

        private string _GPSLocation;

        public string GPSLocation
        {
            get { return _GPSLocation; }
            set { _GPSLocation = value; }
        }

        private string _countryName;

        public string CountryName
        {
            get { return _countryName; }
            set { _countryName = value; }
        }

        private string _photographer;

        public string Photographer
        {
            get { return _photographer; }
            set { _photographer = value; }
        }


    }
}
