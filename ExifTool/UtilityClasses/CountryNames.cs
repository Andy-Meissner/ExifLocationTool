using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace ExifTool.UtilityClasses
{
    public class CountryNames
    {
        public static string GetCountryName(double[] coordinates)
        {
            string countryName = String.Empty;
            double latitude = coordinates[0];
            double longitude = coordinates[1];
            try
            {
                XDocument xml = GetXmLforCoordinates(latitude, longitude);

                if (xml.Root != null) countryName = xml.Root.Descendants("country").FirstOrDefault()?.Value;

                if (countryName == null)
                {
                    return String.Empty;
                }

                return countryName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return String.Empty;
            }
        }

        private static XDocument GetXmLforCoordinates(double latitude, double longitude)
        {
            var locale = CultureInfo.CreateSpecificCulture("en-us");
            string lat = latitude.ToString(locale);
            string lon = longitude.ToString(locale);

            var url = "http://nominatim.openstreetmap.org/reverse?format=xml&lat=" + lat+ "&lon=" + lon +  "&zoom=1&addressdetails=1";
            HttpRequest request = new HttpRequest(url);
            return request.GetXmlDoc();
        }
    }
}
