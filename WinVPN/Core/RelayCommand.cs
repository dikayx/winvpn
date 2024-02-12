using System.Windows.Input;

namespace WinVPN.Core
{
    // Ignore compiler warnings
    #pragma warning disable CS8612
    #pragma warning disable CS8625
    #pragma warning disable CS8767
    internal class RelayCommand(Action<object> execute, Func<object, bool> canExecute = null) : ICommand
    {
        private readonly Action<object> _execute = execute;
        private readonly Func<object, bool> _canExecute = canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }
}
