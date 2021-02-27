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
using ValheimBackup.BO;
using ValheimBackup.FTP;

namespace ValheimBackup
{
    /// <summary>
    /// Interaction logic for ServerFormWindow.xaml
    /// </summary>
    public partial class ServerFormWindow : Window
    {
        // PROPERTIES
        public Server Server
        {
            get
            {
                return this.DataContext as Server;
            }
            set
            {
                this.DataContext = value;
            }
        }

        // CONSTRUCTORS
        public ServerFormWindow()
        {
            InitializeComponent();
            //set initial blank data context. will be overridden if existing server is supplied to constructor.
            this.Server = new Server("New Server", "desc", new FtpConnectionInfo("0.0.0.0", "8000", "user", "pass"), BackupSettings.Default);
            
        }

        public ServerFormWindow(Server context)
        {
            InitializeComponent();
            this.Server = context;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //init end date checkbox
            if(Server.BackupSettings.Schedule.EndDate == null)
            {
                CheckBoxBackupEndDate.IsChecked = true;
            }
        }

        // EVENT HANDLERS
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void CheckBoxBackupEndDate_Checked(object sender, RoutedEventArgs e)
        {
            Server.BackupSettings.Schedule.EndDate = null;
        }

        private void CheckBoxBackupEndDate_Unchecked(object sender, RoutedEventArgs e)
        {
            Server.BackupSettings.Schedule.EndDate = Server.BackupSettings.Schedule.StartDate.Value.AddDays(365);
        }

        private void ButtonTestFtp_Click(object sender, RoutedEventArgs e)
        {
            var res = FtpManager.Test(Server.ConnectionInfo);
            MessageBox.Show(res ? "Connected" : "Unable to connect.");
        }
    }
}
