using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
            var path = GetPath();

            if (!path.Equals(String.Empty))
            {
                DirectoryValidator val = new DirectoryValidator(path);
                List<string> paths = val.GetAllValidPaths();
                var sortedPaths = SortPaths(paths);
                _imagePaths.AddRange(sortedPaths);
            }
            return path;
        }

        public string GetPath()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
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