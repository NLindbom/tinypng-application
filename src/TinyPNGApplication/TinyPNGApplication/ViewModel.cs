using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace TinyPNGApplication
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string apiKey = null;
        private string inputFilename = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public TransformGroup ImageRenderTransform { get; set; }

        public string APIKey
        {
            get { return apiKey; }
            set
            {
                apiKey = value;

                TinifyAPI.Tinify.Key = apiKey;

                Properties.Settings.Default.APIKey = value;
                Properties.Settings.Default.Save();

                NotifyPropertyChanged("APIKey");
            }
        }

        public ViewModel()
        {
            APIKey = Properties.Settings.Default.APIKey;

            ImageRenderTransform = new TransformGroup()
            {
                Children =
                {
                    new ScaleTransform(),
                    new TranslateTransform()
                }
            };
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
