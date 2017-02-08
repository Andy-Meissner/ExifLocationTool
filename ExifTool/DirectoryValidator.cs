using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExifTool
{
    public class DirectoryValidator
    {
        private readonly List<String> _consoleArguments;

        public DirectoryValidator(String[] args)
        {
            _consoleArguments = args.ToList<String>();
        }

        public DirectoryValidator(string path)
        {
            _consoleArguments = new List<string>();
            _consoleArguments.Add(path);
        }

        public List<String> GetAllValidPaths()
        {
            List<String> imgPathList = new List<string>();

            var argumentImgPaths = _consoleArguments.FindAll(IsImage);
            var argumentDirPaths = _consoleArguments.FindAll(IsDirectory);
            var imgPathsFromDir = GetAllImagesPaths(argumentDirPaths);

            imgPathList.AddRange(argumentImgPaths);
            imgPathList.AddRange(imgPathsFromDir);
            return imgPathList;
        }

        private List<String> GetImagesFromDirectory(String path)
        {
            List<String> results = new List<string>();
            var thisdir = Directory.GetFiles(path,"*.jpg", SearchOption.TopDirectoryOnly);
            results.AddRange(thisdir);

            return results;
        }

        private List<String> GetAllImagesPaths(List<String> dirPaths)
        {
            List<String> results = new List<String>();

            foreach (String directory in dirPaths)
            {
                var imgsInDir = GetImagesFromDirectory(directory);
                results.AddRange(imgsInDir);
            }
            return results;
        }

        private bool IsImage(String path)
        {
            bool fileExists = File.Exists(path);
            bool isJpg = path.ToUpperInvariant().EndsWith(".JPG");
            return fileExists && isJpg;
        }

        private bool IsDirectory(String path)
        {
            return Directory.Exists(path);
        }
    }
}
