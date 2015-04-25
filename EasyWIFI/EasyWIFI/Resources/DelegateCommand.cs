using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace EasyWIFI
{
    public class DelegateCommand<T> : ICommand
    {
        public Action<T> _execute;
        public Predicate<T> _canExecute;
        private event EventHandler CanExecuteChangedInternal;

        public void Execute(object parameter)
        {
            _execute((parameter == null) ? default(T) : (T)Convert.ChangeType(parameter, typeof(T)));
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            return _canExecute((parameter == null) ? default(T) : (T)Convert.ChangeType(parameter, typeof(T)));
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChangedInternal != null)
                CanExecuteChangedInternal.Invoke(this, EventArgs.Empty);
        }
    }
}
