using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using System.IO;
using ImgCoordTool;

namespace ExifTool
{
    public class Exif
    {
        public Exif(CustomImage img)
        {
            _thisIMG = img;
        }

        private CustomImage _thisIMG;

        public CustomImage Image
        {
            get { return _thisIMG; }
            set { _thisIMG = value; }
        }
        
        public double[] GetGPSCoordinates()
        {
            var img = _thisIMG;
            double[] coordinates = new double[2];
            var typeLatitudeRef = 0x0001;
            var typeLatitude = 0x0002;
            var typeLongitudeRef = 0x0003;
            var typeLongitude = 0x0004;
            try
            {
                double lat = ExifCoordToDouble(img.Image.GetPropertyItem(typeLatitudeRef), img.Image.GetPropertyItem(typeLatitude));
                double lon = ExifCoordToDouble(img.Image.GetPropertyItem(typeLongitudeRef), img.Image.GetPropertyItem(typeLongitude));

                lat = Math.Round(lat, 6);
                lon = Math.Round(lon, 6);

                coordinates[0] = lat;
                coordinates[1] = lon;
                return coordinates;
            }
            catch
            {
                return null;
            }
        }

        public string GetAutor()
        {
            var img = _thisIMG;
            try
            {
                PropertyItem artistProperty = img.Image.GetPropertyItem(0x013b);
                string name = Encoding.Unicode.GetString(artistProperty.Value);
                return name.Remove(name.Length - 1);
            }
            catch
            {
                return "";
            }
        }
        
        public void saveImage(string destPath)
        {
            string path;
            if (destPath.Equals(String.Empty))
            {
                path = _thisIMG.Path;
            }
            else
            {
                path = destPath + @"\\" + Path.GetFileName(_thisIMG.Path);
            }

            try
            {
                _thisIMG.Image.Save(_thisIMG.Bufferpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                _thisIMG.Image.Dispose();
                File.Delete(_thisIMG.Path);
                File.Move(_thisIMG.Bufferpath, path);
            }
            catch
            {
                return;
            }
        }
        
        public bool SetAutor(string autorName)
        {
            // Type : 1 --> Only type that worked with unicode for the artist property
            try
            {
                PropertyItem item = _thisIMG.Image.PropertyItems[0];
                var dataBytes = Encoding.Unicode.GetBytes(autorName + "\0");
                item = getNewPropertyItem(item, 1, 0x013b, dataBytes);
                _thisIMG.Image.SetPropertyItem(item);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetCountryName(string countryName)
        {
            // Type : 7 --> Only type that worked with unicode for the UserComment property
            try
            {
                PropertyItem item = _thisIMG.Image.PropertyItems[0];
                var dataBytes = convertStringForExif(countryName);

                byte[] unicode = new byte[] { 0x55, 0x4E, 0x49, 0x43, 0x4F, 0x44, 0x45, 0x00 };
                byte[] result = new byte[dataBytes.Length + unicode.Length];

                unicode.CopyTo(result, 0);
                dataBytes.CopyTo(result, 8);

                item = getNewPropertyItem(item, 7, 0x9286, result);
                _thisIMG.Image.SetPropertyItem(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private byte[] convertStringForExif(string value)
        {
            // String needs to be 0-terminated, so an extra char hast to be added
            byte[] bValue = Encoding.Unicode.GetBytes(value);
            return bValue;
        }

        public bool SetGPSCoordinates(double[] coordinates)
        {
            double latitude = coordinates[0];
            double longitude = coordinates[1];

            try
            {
                // set Latitude
                PropertyItem item = _thisIMG.Image.PropertyItems[0];
                var dataBytes = getGPSReference(latitude, true);
                item = getNewPropertyItem(item, 2, 0x0001, dataBytes);
                _thisIMG.Image.SetPropertyItem(item);

                dataBytes = convertCoordinate(latitude);
                item = getNewPropertyItem(item, 5, 0x0002, dataBytes);
                _thisIMG.Image.SetPropertyItem(item);

                // set Longitude:

                dataBytes = getGPSReference(longitude, false);
                item = getNewPropertyItem(item, 2, 0x0003, dataBytes);
                _thisIMG.Image.SetPropertyItem(item);

                dataBytes = convertCoordinate(longitude);
                item = getNewPropertyItem(item, 5, 0x0004, dataBytes);
                _thisIMG.Image.SetPropertyItem(item);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private byte[] convertCoordinate(double coordinateValue)
        {
            coordinateValue = Math.Abs(coordinateValue);
            byte[] bytes = new byte[24];

            double degrees = Math.Floor(coordinateValue);
            double minutes = Math.Floor(60*(coordinateValue - degrees));
            double seconds = 3600 * (coordinateValue - degrees) - 60 * minutes;

            int denominator = 1;

            BitConverter.GetBytes(Convert.ToUInt32(degrees)).CopyTo(bytes, 0);
            BitConverter.GetBytes(denominator).CopyTo(bytes, 4);
            BitConverter.GetBytes(Convert.ToUInt32(minutes)).CopyTo(bytes, 8);
            BitConverter.GetBytes(denominator).CopyTo(bytes, 12);
            TypeConverter.getBytes(seconds).CopyTo(bytes, 16);

            return bytes;
        }

        private byte[] getGPSReference(double coordinateValue, bool isLatitude)
        {
            bool isNegative = coordinateValue < 0;
            char GPSRef;
            byte[] result = new byte[2];

            if (isLatitude)
            {
                GPSRef = isNegative ? 'S' : 'N';
            }
            else
            {
                GPSRef = isNegative ? 'W' : 'E';
            }

            result[0] = Convert.ToByte(GPSRef);
            result[1] = 0;

            return result;
        }
        

        private PropertyItem getNewPropertyItem(PropertyItem item, short type, int id, byte[] value)
        {
            item.Type = type;
            item.Id = id;
            item.Len = value.Length;
            item.Value = value;

            return item;
        }
        private double ExifCoordToDouble(PropertyItem propItemRef, PropertyItem propItem)
        {
            double degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            double degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            double degrees = degreesNumerator / (double)degreesDenominator;

            double minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            double minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            double minutes = minutesNumerator / (double)minutesDenominator;

            double secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            double secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            double seconds = secondsNumerator / (double)secondsDenominator;


            double coordinate = degrees + (minutes / 60d) + (seconds / 3600d);
            string gpsRef = Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, or W
            if (gpsRef == "S" || gpsRef == "W")
                coordinate = coordinate * -1;
            return coordinate;
        }
    }
}
