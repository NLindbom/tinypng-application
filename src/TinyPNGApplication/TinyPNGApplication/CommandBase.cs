using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TinyPNGApplication
{
    public abstract class CommandBase : ICommand
    {
        private readonly Func<bool> canExecute;

        public CommandBase(Func<bool> canExecute = null)
        {
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute.Invoke();
        }

        public abstract void Execute(object parameter);
    }
}
