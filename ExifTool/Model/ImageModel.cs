using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExifTool.Model
{
    public class ImageModel : ModelBase
    {
        public ImageModel()
        {
            _imageSource = new BitmapImage();
            _gpsLocation = String.Empty;
            _countryName = String.Empty;
            _photographer = String.Empty;
            _imagePath = String.Empty;
            ImageClosed = true;
        }

        private ImageSource _imageSource;
        private string _imagePath;
        private string _gpsLocation;
        private string _countryName;
        private string _photographer;
        private bool _imageClosed;

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value; OnPropertyChanged("ImageSource"); }
        }
        
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; OnPropertyChanged("ImagePath"); SetImage();}
        }

        public string GpsLocation
        {
            get { return _gpsLocation; }
            set { _gpsLocation = value; OnPropertyChanged("GpsLocation"); }
        }

        public string CountryName
        {
            get { return _countryName; }
            set { _countryName = value; OnPropertyChanged("CountryName"); }
        }

        public string Photographer
        {
            get { return _photographer; }
            set { _photographer = value; OnPropertyChanged("Photographer"); }
        }

        public bool ImageClosed
        {
            get { return _imageClosed; }
            set { _imageClosed = value; OnPropertyChanged("ImageClosed"); } 
        }

        public void SetData(CustomImage img)
        {
            if (img != null)
            {
                GpsLocation = img.GpsLocation;
                CountryName = img.CountryName;
                Photographer = img.Photographer;
                ImagePath = img.Path;
                ImageClosed = false;
            }
            else
            {
                GpsLocation = String.Empty;
                CountryName = String.Empty;
                Photographer = String.Empty;
                ImagePath = String.Empty;
                ImageClosed = true;
            }
        }

        private void SetImage()
        {
            BitmapImage src = new BitmapImage();

            if (File.Exists(ImagePath))
            {
                src.BeginInit();
                src.UriSource = new Uri(ImagePath, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
            }

            ImageSource = src;
        }

    }
}
