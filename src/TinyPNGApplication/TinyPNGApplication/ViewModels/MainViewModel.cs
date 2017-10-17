using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyPNGApplication.Client;

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
        private byte[] inputBytes;
        private byte[] outputBytes = null;
        private string outputStatusText = null;

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

        public string OutputStatusText
        {
            get { return outputStatusText; }
            set { SetValue(ref outputStatusText, value); }
        }

        public MainViewModel()
        {
            apiKey = Properties.Settings.Default.APIKey;
            client = new Client.TinyPNGClient(apiKey);

            FileOpenCommand = new AsyncCommand(OnFileOpenAsync);
            FileSaveCommand = new Command(OnFileSave, () => SaveEnabled);
            DropCommand = new AsyncCommand(OnDropAsync, () => OpenEnabled);

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

        public async Task OnDropAsync()
        {
            switch (DragDropState)
            {
                case UserControls.DropBorder.State.Drop:
                    await OpenFileAsync(InputFilename);
                    break;
                default:
                    break;
            }
        }

        public async Task OnFileOpenAsync()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Supported images(*.png; *.jpg) | *.png;*.jpg | All files(*.*) | *.*",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == true)
            {
                await OpenFileAsync(dialog.FileName);
            }
        }

        public void OnFileSave()
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PNG image(*.png) | *.png",
            };

            if (dialog.ShowDialog() == true)
            {

            }
        }

        private async Task OpenFileAsync(string filename)
        {
            if (!File.Exists(filename))
                return;

            try
            {
                Mouse.SetCursor(Cursors.Wait);

                this.inputBytes = File.ReadAllBytes(filename);

                InputImage = GetBitmapImage(inputBytes);

                NotifyPropertyChanged("InputImage");

                SaveEnabled = true;

                ViewMode = ViewModes.Image;
            }
            catch
            {
                InputImage = null;
                OutputImage = null;
                outputBytes = null;

                SaveEnabled = false;

                ViewMode = ViewModes.Drop;
            }
            finally
            {
                Mouse.SetCursor(Cursors.None);
            }

            try
            {
                Mouse.SetCursor(Cursors.Wait);

                OutputStatusText = "Shrinking...";

                var response = await client.ShrinkAsync(this.inputBytes);

                if (response is ShrinkResponse)
                {
                    var shrinkResponse = response as ShrinkResponse;

                    OutputStatusText = "Downloading...";

                    response = await client.GetImage(shrinkResponse.Location);

                    if (response is ImageResponse)
                    {
                        outputBytes = (response as ImageResponse).Data;

                        OutputImage = GetBitmapImage(outputBytes);

                        OutputStatusText = null;
                    }
                }

                if (response is ErrorResponse)
                {
                    var errorResponse = response as ErrorResponse;

                    OutputStatusText = $"Error: {errorResponse.ResponseData.Error}; {errorResponse.ResponseData.Message}";

                    return;
                }
            }
            catch (Exception e)
            {
                OutputStatusText = e.Message;
            }
            finally
            {
                Mouse.SetCursor(Cursors.None);
            }
        }

        private static BitmapImage GetBitmapImage(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            var image = new BitmapImage();

            using (var mem = new MemoryStream(data))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();

            return image;
        }
    }
}
