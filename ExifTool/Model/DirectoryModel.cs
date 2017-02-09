using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifTool.Model
{
    public class DirectoryModel : ModelBase
    {
        private string _sourceDirectory;
        private string _destinationDirectory;

        public DirectoryModel()
        {
            _sourceDirectory = String.Empty;
            _destinationDirectory = String.Empty;
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

        public string DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                OnPropertyChanged("DestinationDirectory");
            }
        }
    }
}
