using ns;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ExifTool
{
    class Controller
    {
        private List<string> _imagePaths;
        private IEnumerator<string> _enumerator;
        public Controller(List<string> imagePaths)
        {
            _imagePaths = new List<string>();
            string[] filenames = imagePaths.ToArray();
            NumericComparer ns = new NumericComparer();
            Array.Sort(filenames, ns);
            _imagePaths.AddRange(filenames);
            _enumerator = _imagePaths.GetEnumerator();
        }

        public CustomImage NextPicture()
        {
            if (_enumerator.MoveNext())
            {
                var path = _enumerator.Current;
                return new CustomImage(path);
            }
            else
            {
                return null;
            }
        }
    }
}
