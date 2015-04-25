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
using System.Windows.Shapes;

using MahApps.Metro.Controls;

namespace EasyWIFI
{
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += About_MouseLeftButtonDown;
            this.Deactivated += About_Deactivated;
        }

        private void About_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void About_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
