using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;

namespace EasyWIFI
{
    public partial class MainWindow : MetroWindow
    {
        private Hardcodet.Wpf.TaskbarNotification.TaskbarIcon tb;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            var StartupLocation = System.Windows.SystemParameters.WorkArea;
            this.Left = StartupLocation.Right - this.Width - 10;
            this.Top = StartupLocation.Bottom - this.Height - 10;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tb = (Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)FindResource("NotifyIcon");
        }

        protected void MainWindow_Closed(object sender, EventArgs e)
        {
            tb.Dispose();
        }

        protected void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        protected void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.Focus();
                this.DragMove();
            }
        }
    }
}
