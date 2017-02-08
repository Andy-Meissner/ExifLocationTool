using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifTool
{
    public class ImageModel : ModelBase
    {
        public ImageModel()
        {
            _imagePath = "";
            _gpsLocation = "";
        }
        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; OnPropertyChanged("ImagePath"); }
        }
        
        private string _gpsLocation;

        public string GpsLocation
        {
            get { return _gpsLocation; }
            set { _gpsLocation = value; }
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
