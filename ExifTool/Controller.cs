using ns;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ExifTool
{
    class Controller
    {
        private List<string> _imagePaths;
        private int position = -1;

        public Controller(List<string> imagePaths)
        {
            _imagePaths = new List<string>();
            string[] filenames = imagePaths.ToArray();
            NumericComparer ns = new NumericComparer();
            Array.Sort(filenames, ns);
            _imagePaths.AddRange(filenames);
        }

        public CustomImage NextPicture()
        {
            try
            {
                position++;
                string nextElement = _imagePaths[position];
                return new CustomImage(nextElement);
            }
            catch
            {
                position = -1;
                return null;
            }
        }

        public CustomImage PrevPicture()
        {
            try
            {
                position--;
                string prevElement = _imagePaths[position];
                return new CustomImage(prevElement);
            }
            catch
            {
                position = 0;
                return null;
            }
        }
    }
}
