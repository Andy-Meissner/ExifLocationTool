using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExifTool.BusinessLogic
{
    public class ViewModelController
    {
        private readonly DirectoryControl _directoryController;
        private CustomImage _currentImage;

        public ViewModelController()
        {
            _directoryController = new DirectoryControl();
        }

        public string OpenSourceFolder()
        {
            var directoryPath = _directoryController.OpenDirectory();
            return directoryPath;
        }

        public string OpenDestinationFolder()
        {
            return _directoryController.GetPathFromFolderDialog();
        }

        public CustomImage GetNextImage()
        {
            var nextImage =_directoryController.NextImage();
            return GetImageWithData(nextImage);
        }

        public CustomImage GetPreviousImage()
        {
            var prevImg = _directoryController.PrevImage();
            return GetImageWithData(prevImg);
        }

        private CustomImage GetImageWithData(CustomImage img)
        {
            img?.ReadExifData();
            _currentImage = img;
            return img;
        }

        public void OpenGoogleMapsExecute()
        {
            var location = String.Empty;
            if (_currentImage != null)
            {
                location = _currentImage.GpsLocation;
            }
            System.Diagnostics.Process.Start("https://www.google.de/maps/@" + Regex.Replace(location, @"\s+", "") + ",10z");
        }

        public void SetGpsData(string textboxGpsData)
        {
            if (_currentImage != null)
            {
                _currentImage.GpsLocation = textboxGpsData;
            }
        }

        public void SetCountryName(string textboxCountryName)
        {
            if (_currentImage != null)
            {
                _currentImage.CountryName = textboxCountryName;
            }
        }

        public void SetPhotographer(string textboxPhotographer)
        {
            if (_currentImage != null)
            {
                _currentImage.Photographer = textboxPhotographer;
            }
        }

        public string GetCountryForGps(string textboxGpsData)
        {
            var coordinatesAsDoubles = TypeConverter.GetCoordsFromString(textboxGpsData);
            if (coordinatesAsDoubles != null)
            {
                var countryName = CountryNames.GetCountryName(coordinatesAsDoubles);
                _currentImage.CountryName = countryName;
                return countryName;
            }
            return String.Empty;
        }

        public void SaveImage(string destination)
        {
            _currentImage.SetExifData();
            _currentImage.SaveImage(destination);
        }

        public bool LastImgInDir()
        {
            return _directoryController.LastPicInDir();
        }

        public bool FirstImgInDir()
        {
            return _directoryController.FirstPicInDir();
        }
    }
}
