using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IoT.ViewHardwares.Input
{
    public class Command : ICommand
    {
        private Action ExecuteAction;

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public Command(Action execute)
        {
            ExecuteAction = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction != null)
                ExecuteAction.Invoke();
        }
    }

    public class Command<T> : ICommand
    {
        public Action<T> ExecuteAction;

        private bool onlyonline = false;

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public Command(Action<T> execute)
        {
            ExecuteAction = execute;
        }

        public Command(Action<T> execute, bool OnlyOnline = false)
        {
            ExecuteAction = execute;
            onlyonline = OnlyOnline;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction != null)
            {
                ExecuteAction.Invoke((T)parameter);
            }
            return;
        }
    }
}
