using System;
using System.Windows.Input;
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
