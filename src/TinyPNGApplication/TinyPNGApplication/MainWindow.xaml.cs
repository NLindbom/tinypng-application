using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TinifyAPI;

namespace TinyPNGApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum ViewModes
        {
            Drop,
            Image
        }

        private ViewModes viewMode = ViewModes.Drop;
        
        public ViewModes ViewMode { get { return viewMode; } set { viewMode = value; ShowViewMode(ViewMode); } }

        public MainWindow()
        {
            InitializeComponent();

            ShowViewMode(viewMode);
        }

        private void Handle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files == null || !files.Any())
                    base.OnDrop(e);

                if (OpenFile(files[0]))
                { 
                    ViewMode = ViewModes.Image;

                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);

                        Tinify(files[0]);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Mouse.SetCursor(Cursors.None);
                    }
                }
            }
        }

        private bool OpenFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
                return false;

            BitmapImage image;
            try
            {
                Mouse.SetCursor(Cursors.Wait);

                image = new BitmapImage(new Uri(filename));

                inputImage.Source = image;
            }
            catch
            {
                return false;
            }
            finally
            {
                DropRectangle.Stroke = new SolidColorBrush(Colors.LightGray);

                Mouse.SetCursor(Cursors.None);
            }

            return true;
        }

        private void ShowViewMode(ViewModes viewMode)
        {
            switch (viewMode)
            {
                case ViewModes.Drop:
                    DropGrid.Visibility = Visibility.Visible;
                    ImageGrid.Visibility = Visibility.Hidden;
                    break;
                case ViewModes.Image:
                    DropGrid.Visibility = Visibility.Hidden;
                    ImageGrid.Visibility = Visibility.Visible;
                    break;
            }
        }

        private async void Tinify(string filename)
        {
            TinifyAPI.Tinify.Key = "u07SRpo95lsQao3IoAOKzJj0wojQ9I-t";

            var sourceData = File.ReadAllBytes(filename);
            var resultData = await TinifyAPI.Tinify.FromBuffer(sourceData).ToBuffer();

            if (resultData == null || resultData.Length == 0)
                return;

            var image = new BitmapImage();
            using (var ms = new MemoryStream(resultData))
            {
                ms.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }
            image.Freeze();

            outputImage.Source = image;
        }

        private void Handle_DragOver(object sender, DragEventArgs e)
        {
            DropRectangle.Stroke = new SolidColorBrush(Colors.LightBlue);
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
