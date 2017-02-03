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
        Controller control;
        private Exif exifForCurrentImage;
        private string pathProperty = "DirectoryPath";
        private Image uiImage;

        public MainWindow()
        {
            InitializeComponent();

            // get default folder path from appconfig and load directory
            string path = AppSettings.GetAppSettings(pathProperty);
            label.Content = path;
            loadDirectory(path);
        }

        /// <summary>  
        ///  this function gets called when the open folder button is clicked
        /// </summary>  
        private void open_folder(object sender, RoutedEventArgs e)
        {
            // opens a new folder dialog, starting at the last directory that got called
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = AppSettings.GetAppSettings(pathProperty);
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // load the directory and set the new path in appconfig
                loadDirectory(dialog.SelectedPath);
                AppSettings.SetAppSettings(pathProperty, dialog.SelectedPath);
            } 
        }

        /// <summary>  
        /// this function gets called when the textbox with GPS-coordinates loses keyboard focus
        /// the function tries to get the countryname for the GPS-location and saves it to the countrynametextbox
        /// </summary>  
        private void coordinatesfield_lostfocus(object sender, RoutedEventArgs e)
        {
            double[] coordinates = getCoordsFromTextField();
            if (coordinates != null) Countryname.Text = CountryNames.getCountryName(coordinates);
        }


        /// <summary>
        /// this function loads all image-paths in the given directory and initialises UI with the first image
        /// </summary>
        /// <param name="path">directory path, where the images are stored</param>
        private void loadDirectory(string path)
        {
            label.Content = path;
            Validator val = new Validator(path);
            List<String> imagePaths = val.GetAllValidPaths();
            if (imagePaths.Count == 0) return;

            control = new Controller(imagePaths);
            
            LoadNextImage();
        }

        /// <summary>
        /// loads image on the UI
        /// </summary>
        /// <param name="img"></param>
        private void showImage(CustomImage img)
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
            uiImage = i;
        }

        /// <summary>
        /// updates all textboxes on UI with parameters of image
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="countryName"></param>
        /// <param name="photographer"></param>
        private void updateUIForCurrentIMG(double[] coordinates, string countryName, string photographer)
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
            if (uiImage != null)
            {
                sp.Children.Remove(uiImage);
            }
            showImage(exifForCurrentImage.Image);
            label3.Content = exifForCurrentImage.Image.Path;
        }

        /// <summary>
        /// this function gets called when the confirm button is clicked
        /// it saves images with the new properties and loads the next image in the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_image(object sender, RoutedEventArgs e)
        {
            double[] coords = getCoordsFromTextField();
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
            /*
            exifForCurrentImage.SetAutor(Photographer.Text);
            exifForCurrentImage.SetGPSCoordinates(coords);
            exifForCurrentImage.SetCountryName(Countryname.Text);
            exifForCurrentImage.saveImage();*/
            
            LoadNextImage();
        }

        /// <summary>
        /// formats the coordinates from the GPS-coordinates textbox
        /// </summary>
        /// <returns></returns>
        private double[] getCoordsFromTextField()
        {
            double[] coords = new double[2];
            try
            {
                string[] coordinates = Coordinates.Text.Split(',');
                if (coordinates[0] == "") return null;


                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

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
        private void LoadNextImage()
        {
            var img = control.NextPicture();
            if (img != null)
            {
                exifForCurrentImage = new Exif(img);
                double[] coordinates = exifForCurrentImage.GetGPSCoordinates();
                string artistName = Photographer.Text;
                if (!exifForCurrentImage.GetAutor().Equals(String.Empty))
                {
                    artistName = exifForCurrentImage.GetAutor();
                }

                string countryName = "";
                if (coordinates != null) countryName = CountryNames.getCountryName(coordinates);

                updateUIForCurrentIMG(coordinates, countryName, artistName);
            }
            else
            {
                clearUI();
                uiImage = null;
                exifForCurrentImage = null;
                MessageBox.Show("Keine weiteren Bilder in diesem Verzeichnis");
            }
        }

        private void clearUI()
        {
            if (uiImage != null)
            {
                sp.Children.Remove(uiImage);
            }
            Photographer.Text = "";
            Countryname.Text = "";
            Coordinates.Text = "";
            label3.Content = "";
        }

        private void btn_skipImage(object sender, RoutedEventArgs e)
        {
            LoadNextImage();
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
    }
}