using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using ns;

namespace ExifTool
{
    public class DirectoryControl : ModelBase
    {
        private List<string> _imagePaths;
        private int _position;
        private string _sourceDirectory;
        private string _destinationDirectory;

        public DirectoryControl()
        {
            _imagePaths = new List<string>();
            _sourceDirectory = String.Empty;
            _destinationDirectory = String.Empty;
             _position = -1;
        }

        public void OpenDirectory(string dialogSelectedPath)
        {
            DirectoryValidator val = new DirectoryValidator(dialogSelectedPath);
            _imagePaths = val.GetAllValidPaths();

            var sortedPaths = SortPaths(_imagePaths);
            _imagePaths.AddRange(sortedPaths);

            SetDirectoryProperties(dialogSelectedPath);
        }

        private void SetDirectoryProperties(string dialogSelectedPath)
        {
            this.SourceDirectory = dialogSelectedPath;

            if (this.DestinationDirectory.Equals(String.Empty))
            {
                this.DestinationDirectory = dialogSelectedPath;
            }
        }

        private string[] SortPaths(List<string> imagePaths )
        {
            string[] filenames = _imagePaths.ToArray();
            NumericComparer ns = new NumericComparer();
            Array.Sort(filenames, ns);
            return filenames;
        }

        public string SourceDirectory
        {
            get { return _sourceDirectory; }
            set
            {
                _sourceDirectory = value;
                OnPropertyChanged("SourceDirectory");
            }
        }

        public string DestinationDirectory {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value; 
                OnPropertyChanged("DestinationDirectory");
            }
        }

        public CustomImage NextImage()
        {
            try
            {
                _position++;
                string nextElement = _imagePaths[_position];
                return new CustomImage(nextElement);
            }
            catch
            {
                _position--;
                return null;
            }
        }

        public CustomImage PrevImage()
        {
            try
            {
                _position--;
                string prevElement = _imagePaths[_position];
                return new CustomImage(prevElement);
            }
            catch
            {
                _position++;
                return null;
            }
        }
    }
}