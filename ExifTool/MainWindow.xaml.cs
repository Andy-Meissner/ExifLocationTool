using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExifTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller _control;
        private Exif _exifForCurrentImage;
        private readonly string _pathProperty = "DirectoryPath";
        private string _destinationPath;
        private Image _uiImage;
        private bool _directoryOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            /*
            // get default folder path from appconfig and load directory
            string path = AppSettings.GetAppSettings(_pathProperty);
            label.Content = path;
            LoadDirectory(path);*/
        }       
        
        /// <summary>  
                 /// this function gets called when the textbox with GPS-coordinates loses keyboard focus
                 /// the function tries to get the countryname for the GPS-location and saves it to the countrynametextbox
                 /// </summary>  
        private void coordinatesfield_lostfocus(object sender, RoutedEventArgs e)
        {/*
            double[] coordinates = GetCoordsFromTextField();
            if (coordinates != null) Countryname.Text = CountryNames.getCountryName(coordinates);*/
        }
        /*
        /// <summary>  
        ///  this function gets called when the open folder button is clicked
        /// </summary>  
        private void open_folder(object sender, RoutedEventArgs e)
        {
            // opens a new folder dialog, starting at the last directory that got called
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = AppSettings.GetAppSettings(_pathProperty)
            };
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // load the directory and set the new path in appconfig
                LoadDirectory(dialog.SelectedPath);
                AppSettings.SetAppSettings(_pathProperty, dialog.SelectedPath);
            } 
        }




        /// <summary>
        /// this function loads all image-paths in the given directory and initialises UI with the first image
        /// </summary>
        /// <param name="path">directory path, where the images are stored</param>
        private void LoadDirectory(string path)
        {
            label.Content = path;
            Destinationpath.Content = path;
            _destinationPath = path;

            Validator val = new Validator(path);
            List<String> imagePaths = val.GetAllValidPaths();
            if (imagePaths.Count == 0) return;

            _control = new Controller(imagePaths);
            _directoryOpen = true;
            LoadNextImage(true);
        }

        /// <summary>
        /// loads image on the UI
        /// </summary>
        /// <param name="img"></param>
        private void ShowImage(CustomImage img)
        {
            Image i = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(img.Path, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            i.Source = src;
            i.Stretch = Stretch.UniformToFill;

            sp.Children.Add(i);
            _uiImage = i;
        }

        /// <summary>
        /// updates all textboxes on UI with parameters of image
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="countryName"></param>
        /// <param name="photographer"></param>
        private void UpdateUiForCurrentImg(double[] coordinates, string countryName, string photographer)
        {
            CultureInfo cult = new CultureInfo("en-US");
            if (coordinates != null)
            {
                Coordinates.Text = coordinates[0].ToString(cult) + ", " + coordinates[1].ToString(cult);
            }
            else
            {
                Coordinates.Text = "";
            }
            Countryname.Text = countryName;
            Photographer.Text = photographer;
            if (_uiImage != null)
            {
                sp.Children.Remove(_uiImage);
            }
            ShowImage(_exifForCurrentImage.Image);
            label3.Content = _exifForCurrentImage.Image.Path;
        }

        /// <summary>
        /// this function gets called when the confirm button is clicked
        /// it saves images with the new properties and loads the next image in the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_image(object sender, RoutedEventArgs e)
        {
            double[] coords = GetCoordsFromTextField();
            if (coords == null)
            {
                MessageBox.Show("Speichern nicht möglich: Koordinaten nicht vorhanden oder im falschen Format");
                return;
            }
            else if (Countryname.Text.Length > 40 || Photographer.Text.Length > 40)
            {
                MessageBox.Show("Speichern nicht möglich: Eingabestring zu lang");
                return;
            }
            
            _exifForCurrentImage.SetAutor(Photographer.Text);
            _exifForCurrentImage.SetGPSCoordinates(coords);
            _exifForCurrentImage.SetCountryName(Countryname.Text);
            _exifForCurrentImage.saveImage(_destinationPath);
            
            LoadNextImage(true);
        }

        /// <summary>
        /// formats the coordinates from the GPS-coordinates textbox
        /// </summary>
        /// <returns></returns>
        private double[] GetCoordsFromTextField()
        {
            double[] coords = new double[2];
            try
            {
                string[] coordinates = Coordinates.Text.Split(',');
                if (coordinates[0] == "") return null;


                NumberFormatInfo provider = new NumberFormatInfo {NumberDecimalSeparator = "."};

                coords[0] = Convert.ToDouble(coordinates[0], provider);
                coords[1] = Convert.ToDouble(coordinates[1], provider);
            }
            catch
            {
                return null;
            }
            return coords;
        }

        /// <summary>
        /// this functions loads the next image and metadata that is available
        /// </summary>
        private void LoadNextImage(bool next)
        {
            var img = next ? _control.NextPicture() : _control.PrevPicture();
            if (img != null)
            {
                _exifForCurrentImage = new Exif(img);
                double[] coordinates = _exifForCurrentImage.GetGPSCoordinates();
                string artistName = Photographer.Text;
                if (!_exifForCurrentImage.GetAutor().Equals(String.Empty))
                {
                    artistName = _exifForCurrentImage.GetAutor();
                }

                string countryName = "";
                if (coordinates != null) countryName = CountryNames.getCountryName(coordinates);

                UpdateUiForCurrentImg(coordinates, countryName, artistName);
            }
            else
            {
                if (next)
                {
                    _directoryOpen = false;
                    ClearUi();
                    _uiImage = null;
                    _exifForCurrentImage = null;
                    MessageBox.Show("Keine weiteren Bilder in diesem Verzeichnis");
                }
                else
                {
                    MessageBox.Show("Erstes Bild im Verzeichnis");
                }
            }
        }

        private void ClearUi()
        {
            if (_uiImage != null)
            {
                sp.Children.Remove(_uiImage);
            }
            Photographer.Text = "";
            Countryname.Text = "";
            Coordinates.Text = "";
            label3.Content = "";
        }

        private void btn_skipImage(object sender, RoutedEventArgs e)
        {
            if (_directoryOpen)
            {
                LoadNextImage(true);
            }
        }

        private void btn_prevImage(object sender, RoutedEventArgs e)
        {
            if(_directoryOpen)
            {
                LoadNextImage(false);
            }
        }

        /// <summary>
        /// open google maps on the location given by the GPS-location textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void open_gmaps(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.de/maps/@" + Regex.Replace(Coordinates.Text, @"\s+", "") +  ",10z"); 
        }

        private void btn_setDestination(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = AppSettings.GetAppSettings(_pathProperty)
            };
            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _destinationPath = dialog.SelectedPath;
                Destinationpath.Content = dialog.SelectedPath;
            }
        }*/
    }
}