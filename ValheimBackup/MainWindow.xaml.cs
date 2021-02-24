using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ValheimBackup.BO;
using ValheimBackup.Extensions;

namespace ValheimBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BetterObservableCollection<Server> servers;
        public MainWindow()
        {
            InitializeComponent();
            var sampleServers = new List<Server>
            {
                new Server("Server 1", "server 1 description", new FtpConnectionInfo("127.0.0.1", "5000", "user", "pass"), BackupSettings.Default),
                new Server("Server 2", "server 2 description", new FtpConnectionInfo("198.125.56.87", "64800", "user", "pass"), BackupSettings.Default),
                new Server("Server 3", "server 3 description", new FtpConnectionInfo("10.54.221.9", "16543", "user", "pass"), BackupSettings.Default),
                new Server("Server 4", "server 4 description", new FtpConnectionInfo("10.99.108.224", "9898", "user", "pass"), BackupSettings.Default),
            };
            servers = new BetterObservableCollection<Server>(sampleServers);
            ListServers.ItemsSource = servers;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //init end date checkbox

            //init world selection list
        }

        private void ButtonAddServer_Click(object sender, RoutedEventArgs e)
        {
            ServerFormWindow dialog = new ServerFormWindow();
            var result = dialog.ShowDialog();

            if(result == true)
            {
                servers.Add(dialog.Server);
            }
        }

        private void ButtonRemoveServer_Click(object sender, RoutedEventArgs e)
        {
            //TODO: check if we want to remove existing backup files when deleting.
            var server = ListServers.SelectedItem as Server;

            var result = MessageBox.Show(this, "Do you really want to delete the server: \r\n\"" + server + "\"? \r\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if(result == MessageBoxResult.OK)
            {
                servers.Remove(server);
            }
        }

        private void ButtonEditServer_Click(object sender, RoutedEventArgs e)
        {
            if (ListServers.SelectedItem == null) return;

            var original = ListServers.SelectedItem as Server;
            var copy = original.Copy();

            var result = new ServerFormWindow(copy).ShowDialog();

            if(result == true)
            {
                servers.Replace(original, copy);
            }

            return;
        }
    }
}
