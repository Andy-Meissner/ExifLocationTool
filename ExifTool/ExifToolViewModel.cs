using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ExifTool
{
    public class ExifToolViewModel
    {
        private CustomImage _currentImage;
        private DirectoryControl _currentDir;

        public ExifToolViewModel()
        {

        }

        public ICommand GetSourceFolder
        {
            get
            {
                return new RelayCommand(OpenSourceFolderDialogExecute);
            }
        }

        public DirectoryControl CurrentDir
        {
            get { return _currentDir; }
            set { _currentDir = value; }
        }

        public CustomImage CurrentImage
        {
            get { return _currentImage; }
            set { _currentImage = value; }
        }


        void OpenSourceFolderDialogExecute()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            this.CurrentDir.SourceDirectory = dialog.SelectedPath;
        }
        public ICommand GetDestinationFolder
        {
            get
            {
                return new RelayCommand(OpenDestFolderDialogExecute);
            }
        }

        void OpenDestFolderDialogExecute()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            this.CurrentDir.DestinationDirectory = dialog.SelectedPath;
        }

        public ICommand GetPrevImage
        {
            get
            {
                return new RelayCommand(GetPrevImageExecute);
            }
        }

        void GetPrevImageExecute()
        {
        }

        public ICommand GetNextImage
        {
            get
            {
                return new RelayCommand(GetNextImageExecute);
            }
        }

        void GetNextImageExecute()
        {
        }

        public ICommand SaveImage {
            get
            {
                return new RelayCommand(SaveImageExecute);
            }
        }

        void SaveImageExecute()
        {
        }

        public ICommand OpenGoogleMaps
        {
            get
            {
                return new RelayCommand(OpenGoogleMapsExecute);
            }
        }

        void OpenGoogleMapsExecute()
        {
        }

        public void GetCountryForCoordinates()
        {

        }
    }
}
