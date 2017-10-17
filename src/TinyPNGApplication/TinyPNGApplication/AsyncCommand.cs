using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TinyPNGApplication
{
    public class AsyncCommand : CommandBase
    {
        private readonly Func<Task> command;

        public AsyncCommand(Func<Task> command, Func<bool> canExecute = null)
        {
            this.command = command;
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object parameter)
        {
            return command.Invoke();
        }
    }
}
