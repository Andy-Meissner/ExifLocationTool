using System;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ExifTool
{
    public class HttpRequest
    {
        private String _url;
        public HttpRequest(String url)
        {
            _url = url;
        }

        public XDocument GetXmlDoc()
        {
            string xmlString;

            // Exception muss noch abgefangen werden:

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("Referer", "https://www.google.de/?gws_rd=ssl");
                webClient.Headers.Add("Accept-Language", "de");
                webClient.Headers.Add("Accept-Charset", "utf-8");
                webClient.Encoding = Encoding.UTF8;
                xmlString = webClient.DownloadString(_url);
            }
            
            XDocument xd = XDocument.Parse(xmlString);
            return xd;
        }
    }
}
