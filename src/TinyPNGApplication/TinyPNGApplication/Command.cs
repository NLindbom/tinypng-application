using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TinyPNGApplication
{
    public class Command : CommandBase
    {
        public Action command;

        public Command(Action command, Func<bool> canExecute = null)
        {
            this.command = command;
        }

        public override void Execute(object parameter)
        {
            command.Invoke();
        }
    }
}
