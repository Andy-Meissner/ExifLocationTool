using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifTool
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
    }
}
