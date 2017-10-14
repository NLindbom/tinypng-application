using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TinyPNGApplication
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Action action;
        public Func<bool> canExecute;

        public Command(Action execute, Func<bool> canExecute = null)
        {
            action = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}
