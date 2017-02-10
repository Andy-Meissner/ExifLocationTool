using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using ExifTool.UtilityClasses;

namespace ExifTool.ImageData
{
    public class CustomImage
    {
        public CustomImage(string path)
        {
            Path = path;
            try
            {
                Image = Image.FromFile(Path);
            }
            catch {
                return; 
            }
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

        public bool SaveImage(string directoryPath)
        {
            try
            {
                Image.Save(Bufferpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                Image.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("Bild konnte nicht zwischengespeichert gespeichert werden: " + e.Message);
                return false;
            }
            try
            {
                File.Delete(Path);
                var newPath = directoryPath + @"\" + System.IO.Path.GetFileName(Path);
                File.Move(Bufferpath, newPath);
            }
            catch (Exception e)
            {
                MessageBox.Show("Fehler beim Speichern: " + e.Message);
                return false;
            }
            return true;
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

        public bool SetExifData()
        {
            try
            {
                Image = Exif.SetAutor(Photographer, Image);
                Image = Exif.SetCountryName(CountryName, Image);
                var coordinates = TypeConverter.GetCoordsFromString(GpsLocation);
                Image = Exif.SetGpsCoordinates(coordinates, Image);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Daten fehlerhaft: " + e.Message);
                return false;
            }
        }
    }
}
