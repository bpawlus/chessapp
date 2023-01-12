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

namespace ChessApp
{
    /// <summary>
    /// Logika interakcji dla klasy WindowEditHost.xaml
    /// </summary>
    public partial class WindowEditHost : Window
    {
        private MainWindow owner;

        public WindowEditHost(MainWindow _owner)
        {
            owner = _owner;
            InitializeComponent();
            host.Text = owner.CWAHost;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show($"Chess Web App host changed to: {host.Text}", "Login", button, icon, MessageBoxResult.Yes);
            owner.EditHostCallback(host.Text);
            this.Close();
        }
    }
}
