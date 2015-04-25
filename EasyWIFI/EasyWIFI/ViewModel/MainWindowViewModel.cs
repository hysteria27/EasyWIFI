using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using EasyWIFI.Lib.Host;
using EasyWIFI.Lib.WLAN;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
//using MahApps.Metro;

namespace EasyWIFI
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields...
        private readonly DelegateCommand<object> _startHotspot;
        private ObservableCollection<HostConnection> _host;
        private HostConnection _selectedHost;
        private string _ssid, _key, _startHotspot_Content;
        private bool _isFieldEnabled, _isInProcess;
        private Function Validate = new Function();
        private EasyWIFIHost WIFI = new EasyWIFIHost();
        private SharableConnection SharedConnection = new SharableConnection();
        private WlanManager Manager = new WlanManager();
        private BackgroundWorker Worker = new BackgroundWorker();
        private AppViewModel AVM = new AppViewModel();
        #endregion

        public MainWindowViewModel()
        {
            GetHost();
            IsFieldEnabled = true;
            StartHotspot_Content = "Start";

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsInProcess = false;
                SSID = Properties.Settings.Default.SSID;
                KEY = Properties.Settings.Default.KEY;
            }));

            _startHotspot = new DelegateCommand<object>
            {
                _execute = (s) =>
                {
                    if (!WIFI.IsStarted())
                        StartHotspot();
                    else
                        StopHotspot();
                },
                _canExecute = (s) =>
                {
                    bool canExecute;
                    if (!WIFI.IsStarted())
                        canExecute = Validate.Validate(this.SSID, this.KEY) && !IsInProcess;
                    else
                        canExecute = true && !IsInProcess;
                    return canExecute;
                }
            };

            Worker.DoWork += Worker_DoWork;
            Manager.HostedNetworkStarted += Manager_HostedNetworkStarted;
            Manager.HostedNetworkStopped += Manager_HostedNetworkStopped;
        }

        #region Properties...
        public string SSID
        {
            get { return _ssid; }
            set
            {
                if (value != _ssid)
                {
                    _ssid = value;
                    NotifyPropertyChanged("SSID");
                    _startHotspot.OnCanExecuteChanged();
                }
            }
        }

        public string KEY
        {
            get { return _key; }
            set
            {
                if (value != _key)
                {
                    _key = value;
                    NotifyPropertyChanged("KEY");
                    _startHotspot.OnCanExecuteChanged();
                }
            }
        }

        public bool IsFieldEnabled
        {
            get { return _isFieldEnabled; }
            set
            {
                if (value != _isFieldEnabled)
                {
                    _isFieldEnabled = value;
                    NotifyPropertyChanged("IsFieldEnabled");
                }
            }
        }

        public bool IsInProcess
        {
            get { return _isInProcess; }
            set
            {
                if (value != _isInProcess)
                {
                    _isInProcess = value;
                    NotifyPropertyChanged("IsInProcess");
                    _startHotspot.OnCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<HostConnection> Host
        {
            get { return _host; }
            set
            {
                if (value != _host)
                {
                    _host = value;
                    NotifyPropertyChanged("Host");
                }
            }
        }

        public HostConnection SelectedHost
        {
            get { return _selectedHost; }
            set
            {
                if (value != _selectedHost)
                {
                    _selectedHost = value;
                    NotifyPropertyChanged("SelectedHost");
                }
            }
        }

        public string StartHotspot_Content
        {
            get { return _startHotspot_Content; }
            set
            {
                if (value != _startHotspot_Content)
                {
                    _startHotspot_Content = value;
                    NotifyPropertyChanged("StartHotspot_Content");
                }
            }
        }
        #endregion

        public ICommand StartHotspot_Click
        {
            get { return _startHotspot; }
        }

        public ICommand RefreshHostConnection
        {
            get { return new DelegateCommand<object> { _execute = (s) => GetHost() }; }
        }

        public ICommand AboutApplication
        {
            get { return AVM.AboutApplication; }
        }

        #region Method...
        public void GetHost()
        {
            Host = new ObservableCollection<HostConnection>();
            foreach (SharableConnection connections in WIFI.GetSharableConnections())
            {
                Host.Add(new HostConnection() { Name = connections.Name });
            }
            SelectedHost = Host[0];
        }

        public void StartHotspot()
        {
            IsFieldEnabled = false;
            IsInProcess = true;
            StartHotspot_Content = "Starting...";
            Worker.RunWorkerAsync("Start");
        }

        public void StopHotspot()
        {
            IsInProcess = true;
            StartHotspot_Content = "Stopping...";
            Worker.RunWorkerAsync("Stop");
        }

        private void UpdateUI()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsInProcess = false;

                if (WIFI.IsStarted())
                {
                    StartHotspot_Content = "Stop";
                    AVM.ChangeIconApplication("active");
                }
                else
                {
                    IsFieldEnabled = true;
                    StartHotspot_Content = "Start";
                    AVM.ChangeIconApplication("inactive");
                }
            }));
        }
        #endregion

        #region Events...
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((string)e.Argument == "Start")
            {
                foreach (SharableConnection connections in WIFI.GetSharableConnections())
                {
                    if (connections.Name == SelectedHost.Name)
                    {
                        SharedConnection.Name = connections.Name;
                        SharedConnection.DeviceName = connections.DeviceName;
                        SharedConnection.Guid = connections.Guid;

                        WIFI.SetConnectionSettings(SSID, 10);
                        WIFI.SetPassword(KEY);

                        System.Threading.Thread.Sleep(7000);
                        WIFI.Start((SharableConnection)SharedConnection);
                    }
                }
            }
            else if ((string)e.Argument == "Stop")
            {
                System.Threading.Thread.Sleep(3000);
                WIFI.Stop();
            }
            else { }
        }

        private void Manager_HostedNetworkStarted(object sender, EventArgs e)
        {
            UpdateUI();
            //try
            //{
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        IsInProcess = false;
            //        StartHotspot_Content = "Stop";
            //        //AVM.ChangeIconApplication("active.ico");
            //        //ThemeManager.ChangeAppStyle(Application.Current,
            //        //                            ThemeManager.GetAccent("Orange"),
            //        //                            ThemeManager.GetAppTheme("BaseLight"));
            //    }));
            //}
            //catch { }
        }

        private void Manager_HostedNetworkStopped(object sender, EventArgs e)
        {
            UpdateUI();
            //try
            //{
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        IsInProcess = false;
            //        EnabledField = true;
            //        StartHotspot_Content = "Start";
            //        //AVM.ChangeIconApplication("inactive.ico");
            //        //ThemeManager.ChangeAppStyle(Application.Current,
            //        //                            ThemeManager.GetAccent("Blue"),
            //        //                            ThemeManager.GetAppTheme("BaseLight"));
            //    }));
            //}
            //catch { }
        }
        #endregion
    }

    public class HostConnection
    {
        public string Name { get; set; }
    }

    public class Function
    {
        public bool Validate(string SSID, string KEY)
        {
            bool valid;
            if (!string.IsNullOrEmpty(SSID) && !string.IsNullOrEmpty(KEY) && SSID.Length >= 4 && KEY.Length >= 8 && SSID.Length <= 32 && KEY.Length <= 64)
                valid = true;
            else
                valid = false;
            return valid;
        }
    }
}
