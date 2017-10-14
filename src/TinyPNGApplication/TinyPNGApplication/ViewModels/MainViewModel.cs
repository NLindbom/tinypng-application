using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TinyPNGApplication.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public enum ViewModes
        {
            Drop,
            Image
        }

        private string apiKey;
        private bool openEnabled;
        private bool saveEnabled;
        private Client.TinyPNGClient client;
        private UserControls.DropBorder.State dragDropState;
        private Visibility dropGridVisibility;
        private Visibility imageGridVisibility;

        public string APIKey
        {
            get { return apiKey; }
            set
            {
                if (SetValue(ref apiKey, value))
                {
                    client.APIKey = APIKey;
                    Properties.Settings.Default.APIKey = APIKey;
                    Properties.Settings.Default.Save();
                }

                if (string.IsNullOrWhiteSpace(APIKey))
                    OpenEnabled = true;
            }
        }

        public TransformGroup ImageRenderTransform { get; }

        public UserControls.DropBorder.State DragDropState
        {
            get { return dragDropState; }
            set
            {
                SetValue(ref dragDropState, value);
            }
        }

        public string InputFilename { get; set; }

        public bool SaveEnabled
        {
            get { return saveEnabled; }
            set { SetValue(ref saveEnabled, value); }
        }

        public bool OpenEnabled
        {
            get { return openEnabled; }
            set { SetValue(ref openEnabled, value); }
        }

        public ICommand FileOpenCommand { get; set; }
        public ICommand FileSaveCommand { get; set; }
        public ICommand DropCommand { get; set; }

        public ImageSource InputImage { get; set; }
        public ImageSource OutputImage { get; set; }

        public SolidColorBrush DropRectangleBrush
        {
            get
            {
                SolidColorBrush brush; 

                if (DragDropState == UserControls.DropBorder.State.DragOver)
                {
                    brush = new SolidColorBrush(Colors.LightBlue);
                }

                brush = new SolidColorBrush(Colors.LightGray);

                return brush;
            }
        }

        public Visibility DropGridVisibility
        {
            get { return dropGridVisibility; }
            set { SetValue(ref dropGridVisibility, value); }
        }

        public Visibility ImageGridVisibility
        {
            get { return imageGridVisibility; }
            set { SetValue(ref imageGridVisibility, value); }
        }

        public ViewModes ViewMode
        {
            set
            {
                switch (value)
                {
                    case ViewModes.Drop:
                        DropGridVisibility = Visibility.Visible;
                        ImageGridVisibility = Visibility.Hidden;
                        break;
                    case ViewModes.Image:
                        DropGridVisibility = Visibility.Hidden;
                        ImageGridVisibility = Visibility.Visible;
                        break;
                }
            }
        }

        public MainViewModel()
        {
            apiKey = Properties.Settings.Default.APIKey;
            client = new Client.TinyPNGClient(apiKey);

            FileOpenCommand = new Command(OnFileOpen);
            FileSaveCommand = new Command(OnFileSave, () => SaveEnabled);
            DropCommand = new Command(OnDrop, () => OpenEnabled);

            OpenEnabled = true;
            SaveEnabled = true;

            ImageRenderTransform = new TransformGroup()
            {
                Children =
                    {
                        new ScaleTransform(),
                        new TranslateTransform()
                    }
            };

            ViewMode = ViewModes.Drop;
        }

        public void OnDrop()
        {
            switch (DragDropState)
            {
                case UserControls.DropBorder.State.DragOver:
                    break;
                case UserControls.DropBorder.State.Drop:
                    if (OpenFile(InputFilename))
                    { 
                        ViewMode = ViewModes.Image;
                    }
                    break;
                default:
                    break;
            }

            NotifyPropertyChanged("DropRectangleBrush");
        }

        public void OnFileOpen()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                if (OpenFile(dialog.FileName))
                {
                    ViewMode = ViewModes.Image;
                }
            }
        }

        public void OnFileSave()
        {
        }

        private bool OpenFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
                return false;

            try
            {
                Mouse.SetCursor(Cursors.Wait);

                InputImage = new BitmapImage(new Uri(filename));

                NotifyPropertyChanged("InputImage");

                SaveEnabled = true;
            }
            catch
            {
                InputImage = null;

                SaveEnabled = false;

                ViewMode = ViewModes.Drop;

                return false;
            }
            finally
            {
                Mouse.SetCursor(Cursors.None);
            }


            return true;
        }
    }
}
