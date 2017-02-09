using System;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace ExifTool
{
    public class CustomImage
    {
        public CustomImage(string path)
        {
            Path = path;
            Image = Image.FromFile(Path);
            Photographer = String.Empty;
            CountryName = String.Empty;;
            GpsLocation = String.Empty;
            Bufferpath = System.IO.Path.GetDirectoryName(Path) + "\\new_" + System.IO.Path.GetFileName(Path);
        }

        public Image Image { get; set; }

        public string Path { get; }

        public string Bufferpath { get; }

        public string GpsLocation { get; set; }


        public string CountryName { get; set; }

        public string Photographer { get; set; }

        public void SaveImage(string directoryPath)
        {
            try
            {
                Image.Save(Bufferpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                Image.Dispose();
                var newPath = directoryPath + @"\" + System.IO.Path.GetFileName(Path);
                File.Delete(Path);
                File.Move(Bufferpath, newPath);
            }
            catch 
            {
                return;
            }
        }

        public void ReadExifData()
        {
            CultureInfo cult = new CultureInfo("en-US");
            Photographer = Exif.GetAutor(this.Image);
            var coordinates = Exif.GetGpsCoordinates(this.Image);
            if (coordinates != null)
            {
                GpsLocation = coordinates[0].ToString(cult) + "," + coordinates[1].ToString(cult);
                CountryName = CountryNames.GetCountryName(coordinates);
            }
        }

        public void SetExifData()
        {
            Image = Exif.SetAutor(Photographer, Image);
            Image = Exif.SetCountryName(CountryName, Image);
            var coordinates = TypeConverter.GetCoordsFromString(GpsLocation);
            Image = Exif.SetGpsCoordinates(coordinates,Image);
        }
    }
}
