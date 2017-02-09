using System;
using System.Windows.Input;
using ExifTool.BusinessLogic;
using ExifTool.Model;

namespace ExifTool
{
    public class ExifToolViewModel
    {
        private readonly Controller _controller;
        
        public ExifToolViewModel()
        {
            DirectoryModel = new DirectoryModel();
            ImageModel = new ImageModel();
            _controller = new Controller();
            ImageModel.PropertyChanged += ImageModel_PropertyChanged;
        }

        public DirectoryModel DirectoryModel { get; }

        public ImageModel ImageModel { get; }

        public ICommand GetSourceFolder => new RelayCommand(OpenSourceFolderExecute);

        void OpenSourceFolderExecute()
        {
            var directoryPath = _controller.OpenSourceFolder();
            DirectoryModel.SourceDirectory = directoryPath;

            var firstImage = _controller.GetNextImage();
            ImageModel.SetData(firstImage);
            
            DirectoryModel.DestinationDirectory = directoryPath;
        }

        public ICommand GetDestinationFolder => new RelayCommand(OpenDestinationFolderExecute, OpenDestinationFolderExecuteable);

        void OpenDestinationFolderExecute()
        {
            var directoryPath = _controller.OpenDestinationFolder();
            DirectoryModel.DestinationDirectory = directoryPath;
        }

        bool OpenDestinationFolderExecuteable()
        {
            return true;
        }

        public ICommand GetPrevImage => new RelayCommand(GetPrevImageExecute, GetPrevImageExecuteable);

        void GetPrevImageExecute()
        {
            var prevImage = _controller.GetPreviousImage();
            ImageModel.SetData(prevImage);
        }

        bool GetPrevImageExecuteable()
        {
            return true;
        }

        public ICommand GetNextImage => new RelayCommand(GetNextImageExecute, GetNextImageExecuteable);

        void GetNextImageExecute()
        {
            var nextImage = _controller.GetNextImage();
            ImageModel.SetData(nextImage);
        }

        bool GetNextImageExecuteable()
        {
            return true;
        }

        public ICommand SaveImage => new RelayCommand(SaveImageExecute, SaveImageExecuteable);

        void SaveImageExecute()
        {
            _controller.SaveImage(DirectoryModel.DestinationDirectory);
            GetNextImageExecute();
        }

        bool SaveImageExecuteable()
        {
            return true;
        }

        public ICommand OpenGoogleMaps => new RelayCommand(_controller.OpenGoogleMapsExecute);

        public void ImageModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GpsLocation")
            {
                _controller.SetGpsData(ImageModel.GpsLocation);
                ImageModel.CountryName = _controller.GetCountryForGps(ImageModel.GpsLocation);
            }
            else if (e.PropertyName == "Photographer")
            {
                _controller.SetPhotographer(ImageModel.Photographer);
            }
            else if (e.PropertyName == "CountryName")
            {
                _controller.SetCountryName(ImageModel.CountryName);
            }
        }
    }
}
