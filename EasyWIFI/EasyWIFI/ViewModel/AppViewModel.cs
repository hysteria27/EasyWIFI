using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;

namespace EasyWIFI
{
    public class AppViewModel : ViewModelBase
    {
        private string _iconApplication = "pack://application:,,,/Resources/Icons/inactive.ico";

        public string IconApplication
        {
            get { return _iconApplication; }
            set
            {
                _iconApplication = value;
                NotifyPropertyChanged("IconApplication");
            }
        }

        public void ChangeIconApplication(string IconName)
        {
            if (IconName == "active" || IconName == "inactive")
            {
                (((Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)Application.Current.FindResource("NotifyIcon")).DataContext
                    as AppViewModel).IconApplication = "pack://application:,,,/Resources/Icons/" + IconName + ".ico";
            }
        }

        public ICommand ShowHideApplication
        {
            get
            {
                return new DelegateCommand<object>
                {
                    _execute = (s) => Application.Current.MainWindow.Show()
                };
            }
        }

        public ICommand ExitApplication
        {
            get { return new DelegateCommand<object> { _execute = (s) => Application.Current.Shutdown() }; }
        }

        public ICommand AboutApplication
        {
            get
            {
                return new DelegateCommand<object>
                {
                    _execute = (s) =>
                    {
                        if (!Application.Current.Windows.OfType<Window>().Any(w => w.Name.Equals("WindowAbout")))
                        {
                            About About = new About();
                            About.Show();
                        }
                    }
                };
            }
        }
    }
}
