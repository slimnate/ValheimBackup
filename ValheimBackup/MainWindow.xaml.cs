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
using ValheimBackup.Data;
using ValheimBackup.Extensions;
using ValheimBackup.FTP;

namespace ValheimBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.Servers;
        }

        private void ButtonAddServer_Click(object sender, RoutedEventArgs e)
        {
            ServerFormWindow dialog = new ServerFormWindow();
            var result = dialog.ShowDialog();

            if(result == true)
            {
                App.Servers.Add(dialog.Server);
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
                App.Servers.Remove(server);
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
                App.Servers.Replace(original, copy);
            }

            return;
        }

        private void ButtonBackupServer_Click(object sender, RoutedEventArgs e)
        {
            if (ListServers.SelectedItem == null) return;

            var server = ListServers.SelectedItem as Server;

            var files = FtpManager.DownloadWorldFiles(server);

            var backups = DataManager.BackupFtpFiles(server, files);

            foreach(var b in backups)
            {
                App.Backups.Add(b);
            }

            //save files to backup location
            return;
        }
    }
}
