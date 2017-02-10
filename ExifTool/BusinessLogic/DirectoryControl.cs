using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ExifTool.ImageData;
using ExifTool.UtilityClasses;
using ns;

namespace ExifTool.BusinessLogic
{
    public class DirectoryControl
    {
        private List<string> _imagePaths;
        private int _position;

        public DirectoryControl()
        {
            _imagePaths = new List<string>();
            _position = -1;
        }

        public string OpenDirectory()
        {
            var path = GetPathFromFolderDialog();

            if (!path.Equals(String.Empty))
            {
                _imagePaths = new List<string>();
                DirectoryValidator val = new DirectoryValidator(path);
                List<string> paths = val.GetAllValidPaths();
                var sortedPaths = SortPaths(paths);
                _imagePaths.AddRange(sortedPaths);
                _position = -1;
            }
            return path;
        }

        public string GetPathFromFolderDialog()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            string path = AppSettings.GetAppSettings("DirectoryPath");
            dialog.SelectedPath = path;

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                AppSettings.SetAppSettings("DirectoryPath",dialog.SelectedPath);
                return dialog.SelectedPath;
            }
            else
            {
                return String.Empty;
            }
        }


        private string[] SortPaths(List<string> imagePaths)
        {
            string[] filenames = imagePaths.ToArray();
            NumericComparer ns = new NumericComparer();
            Array.Sort(filenames, ns);
            return filenames;
        }

        public CustomImage NextImage()
        {
            var nextPos = _position + 1;
            if (nextPos < _imagePaths.Count)
            {
                _position++;
                string nextElement = _imagePaths[_position];
                return new CustomImage(nextElement);
            }
            return null;
        }

        public CustomImage PrevImage()
        {
            var prevPos = _position - 1;
            if (prevPos > -1)
            {
                _position--;
                string prevElement = _imagePaths[_position];
                return new CustomImage(prevElement);
            }
            return null;
        }

        public bool LastPicInDir()
        {
            if (_imagePaths.Count == 0) return true;
            if (_position + 1 == _imagePaths.Count)
            {
                return true;
            }
            return false;
        }

        public bool FirstPicInDir()
        {
            if (_imagePaths.Count == 0) return true;
            if (_position == 0)
            {
                return true;
            }
            return false;
        }
    }
}