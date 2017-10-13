using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TinyPNGApplication
{
    public abstract class BaseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Func<bool> canExecute;

        public BaseCommand(Func<bool> canExecute)
        {
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute.Invoke();
        }

        public abstract void Execute(object parameter);
    }

    public class Command : BaseCommand
    {
        private Action action;

        public Command(Action execute, Func<bool> canExecute = null) : base(canExecute)
        {
            action = execute;
        }

        public override void Execute(object parameter)
        {
            action.Invoke();
        }
    }

    public class Command<T> : BaseCommand where T : class
    {
        private Action<T> action;

        public Command(Action<T> execute, Func<bool> canExecute = null) : base(canExecute)
        {
            action = execute;
        }

        public override void Execute(object parameter)
        {
            action.Invoke(parameter as T);
        }
    }
}
