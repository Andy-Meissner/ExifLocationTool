using System;
using System.Globalization;

namespace ExifTool.UtilityClasses
{
    class TypeConverter
    {
        public static byte[] getBytes(double value)
        {
            byte[] nomByte = new byte[4];
            byte[] denomByte = new byte[4];
            byte[] result = new byte[8];

            uint nominator = Convert.ToUInt32(Math.Truncate(Math.Abs(value) * 1000000));
            uint denominator = 1000000;

            nomByte = BitConverter.GetBytes(nominator);
            denomByte = BitConverter.GetBytes(denominator);

            nomByte.CopyTo(result, 0);
            denomByte.CopyTo(result, 4);
            return result;
        }

        public static double[] GetCoordsFromString(string coordinatesAsString)
        {
            double[] coords = new double[2];
            try
            {
                string[] coordinates = coordinatesAsString.Split(',');
                if (coordinates[0] == "") return null;


                NumberFormatInfo provider = new NumberFormatInfo { NumberDecimalSeparator = "." };

                coords[0] = Convert.ToDouble(coordinates[0], provider);
                coords[1] = Convert.ToDouble(coordinates[1], provider);
            }
            catch
            {
                return null;
            }
            return coords;
        }
    }
}
