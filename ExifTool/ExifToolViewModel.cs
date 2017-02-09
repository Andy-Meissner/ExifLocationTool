using System;
using System.Windows.Input;
using ExifTool.BusinessLogic;
using ExifTool.Model;

namespace ExifTool
{
    public class ExifToolViewModel
    {
        private readonly ViewModelController _viewModelController;
        
        public ExifToolViewModel()
        {
            DirectoryModel = new DirectoryModel();
            ImageModel = new ImageModel();
            _viewModelController = new ViewModelController();
            ImageModel.PropertyChanged += ImageModel_PropertyChanged;
        }

        public DirectoryModel DirectoryModel { get; }

        public ImageModel ImageModel { get; }

        public ICommand GetSourceFolder => new RelayCommand(OpenSourceFolderExecute);

        void OpenSourceFolderExecute()
        {
            var directoryPath = _viewModelController.OpenSourceFolder();
            DirectoryModel.SourceDirectory = directoryPath;

            var firstImage = _viewModelController.GetNextImage();
            ImageModel.SetData(firstImage);
            
            DirectoryModel.DestinationDirectory = directoryPath;
        }

        public ICommand GetDestinationFolder => new RelayCommand(OpenDestinationFolderExecute, OpenDestinationFolderExecuteable);

        void OpenDestinationFolderExecute()
        {
            var directoryPath = _viewModelController.OpenDestinationFolder();
            DirectoryModel.DestinationDirectory = directoryPath;
        }

        bool OpenDestinationFolderExecuteable()
        {
            return true;
        }

        public ICommand GetPrevImage => new RelayCommand(GetPrevImageExecute, GetPrevImageExecuteable);

        void GetPrevImageExecute()
        {
            var prevImage = _viewModelController.GetPreviousImage();
            ImageModel.SetData(prevImage);
        }

        bool GetPrevImageExecuteable()
        {
            return !_viewModelController.FirstImgInDir();
        }

        public ICommand GetNextImage => new RelayCommand(GetNextImageExecute, GetNextImageExecuteable);

        void GetNextImageExecute()
        {
            var nextImage = _viewModelController.GetNextImage();
            ImageModel.SetData(nextImage);
        }

        bool GetNextImageExecuteable()
        {
            return !_viewModelController.LastImgInDir();
        }

        public ICommand SaveImage => new RelayCommand(SaveImageExecute, SaveImageExecuteable);

        void SaveImageExecute()
        {
            _viewModelController.SaveImage(DirectoryModel.DestinationDirectory);
            GetNextImageExecute();
        }

        bool SaveImageExecuteable()
        {
            return !ImageModel.ImageClosed;
        }

        public ICommand OpenGoogleMaps => new RelayCommand(_viewModelController.OpenGoogleMapsExecute);

        public void ImageModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GpsLocation")
            {
                _viewModelController.SetGpsData(ImageModel.GpsLocation);
                ImageModel.CountryName = _viewModelController.GetCountryForGps(ImageModel.GpsLocation);
            }
            else if (e.PropertyName == "Photographer")
            {
                _viewModelController.SetPhotographer(ImageModel.Photographer);
            }
            else if (e.PropertyName == "CountryName")
            {
                _viewModelController.SetCountryName(ImageModel.CountryName);
            }
        }
    }
}
