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
    public static class Exif
    {
        public static double[] GetGpsCoordinates(Image img)
        {
            double[] coordinates = new double[2];
            var typeLatitudeRef = 0x0001;
            var typeLatitude = 0x0002;
            var typeLongitudeRef = 0x0003;
            var typeLongitude = 0x0004;
            try
            {
                double lat = ExifCoordToDouble(img.GetPropertyItem(typeLatitudeRef), img.GetPropertyItem(typeLatitude));
                double lon = ExifCoordToDouble(img.GetPropertyItem(typeLongitudeRef), img.GetPropertyItem(typeLongitude));

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

        public static string GetAutor(Image img)
        {
            try
            {
                PropertyItem artistProperty = img.GetPropertyItem(0x013b);
                string name = Encoding.Unicode.GetString(artistProperty.Value);
                return name.Remove(name.Length - 1);
            }
            catch
            {
                return String.Empty;
            }
        }
        

        
        public static Image SetAutor(string autorName, Image img)
        {
            // Type : 1 --> Only type that worked with unicode for the artist property
            try
            {
                var image = img;
                PropertyItem item = image.PropertyItems[0];
                var dataBytes = Encoding.Unicode.GetBytes(autorName + "\0");
                item = GetNewPropertyItem(item, 1, 0x013b, dataBytes);
                image.SetPropertyItem(item);
                return image;
            }
            catch
            {
                return img;
            }
        }

        public static Image SetCountryName(string countryName, Image img)
        {
            // Type : 7 --> Only type that worked with unicode for the UserComment property
            try
            {
                var image = img;
                PropertyItem item = image.PropertyItems[0];
                var dataBytes = ConvertStringForExif(countryName);

                byte[] unicode = new byte[] { 0x55, 0x4E, 0x49, 0x43, 0x4F, 0x44, 0x45, 0x00 };
                byte[] result = new byte[dataBytes.Length + unicode.Length];

                unicode.CopyTo(result, 0);
                dataBytes.CopyTo(result, 8);

                item = GetNewPropertyItem(item, 7, 0x9286, result);
                image.SetPropertyItem(item);
                return image;
            }
            catch
            {
                return img;
            }
        }

        private static byte[] ConvertStringForExif(string value)
        {
            // String needs to be 0-terminated, so an extra char hast to be added
            byte[] bValue = Encoding.Unicode.GetBytes(value);
            return bValue;
        }

        public static Image SetGpsCoordinates(double[] coordinates, Image img)
        {
            double latitude = coordinates[0];
            double longitude = coordinates[1];

            try
            {
                var image = img;
                // set Latitude
                PropertyItem item = image.PropertyItems[0];
                var dataBytes = GetGpsReference(latitude, true);
                item = GetNewPropertyItem(item, 2, 0x0001, dataBytes);
                image.SetPropertyItem(item);

                dataBytes = ConvertCoordinate(latitude);
                item = GetNewPropertyItem(item, 5, 0x0002, dataBytes);
                image.SetPropertyItem(item);

                // set Longitude:

                dataBytes = GetGpsReference(longitude, false);
                item = GetNewPropertyItem(item, 2, 0x0003, dataBytes);
                image.SetPropertyItem(item);

                dataBytes = ConvertCoordinate(longitude);
                item = GetNewPropertyItem(item, 5, 0x0004, dataBytes);
                image.SetPropertyItem(item);
                return image;
            }
            catch
            {
                return img;
            }
        }

        private static byte[] ConvertCoordinate(double coordinateValue)
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

        private static byte[] GetGpsReference(double coordinateValue, bool isLatitude)
        {
            bool isNegative = coordinateValue < 0;
            char gpsRef;
            byte[] result = new byte[2];

            if (isLatitude)
            {
                gpsRef = isNegative ? 'S' : 'N';
            }
            else
            {
                gpsRef = isNegative ? 'W' : 'E';
            }

            result[0] = Convert.ToByte(gpsRef);
            result[1] = 0;

            return result;
        }
        

        private static PropertyItem GetNewPropertyItem(PropertyItem item, short type, int id, byte[] value)
        {
            item.Type = type;
            item.Id = id;
            item.Len = value.Length;
            item.Value = value;

            return item;
        }
        private static double ExifCoordToDouble(PropertyItem propItemRef, PropertyItem propItem)
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
