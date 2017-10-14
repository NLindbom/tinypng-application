using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TinyPNGApplication.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetValue<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (field == null)
            {
                if (value == null)
                    return false;
            }
            else if (field.Equals(value))
            {
                return false;
            }

            field = value;

            NotifyPropertyChanged(propertyName);

            return true;
        }

        private static bool IsObjectEqual<T>(T target, T value)
        {
            if (target == null)
                return value == null;
            return target.Equals(value);
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
