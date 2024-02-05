using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinVPN.Core;
using WinVPN.MVVM.Model;

namespace WinVPN.MVVM.ViewModel
{
    internal class ProtectionViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<ServerModel> Servers { get; set; }
        private ConnectionService _connectionService;

        // TODO: Make status an enum
        private string _connectionStatus;
        public string ConnectionStatus
        {
            get => _connectionStatus ?? "Not connected";
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand ConnectCommand { get; set; }

        private ServerModel _selectedServer;

        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                _selectedServer = value;
                OnPropertyChanged();
                OnServerSelected();
            }
        }

        private string _username;

        public string Username
        {
            get => _username ?? "vpnbook"; // Default username
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public SecureString Password { private get; set; }

        private string _connectBtnContent = "Connect";

        public string ConnectBtnContent
        {
            get => _connectBtnContent;
            set
            {
                if (_connectBtnContent != value)
                {
                    _connectBtnContent = value;
                    OnPropertyChanged(nameof(ConnectBtnContent));
                }
            }
        }

        public ProtectionViewModel()
        {
            // Create a new ConnectionService
            _connectionService = new ConnectionService();
            Servers = _connectionService.GetServers();

            ConnectCommand = new RelayCommand(o =>
            {
                Task.Run(() =>
                {
                    if (SelectedServer == null)
                    {
                        MessageBox.Show("Please select a server!");
                        return;
                    }

                    string passString = new System.Net.NetworkCredential(string.Empty, Password).Password;

                    ConnectionStatus = "Connecting...";
                    ConnectBtnContent = "Connecting...";

                    var process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                    string connectionString = $"/c rasdial {SelectedServer.Id} {Username} {passString} /phonebook:./VPN/{SelectedServer.Id}.pbk";
                    Debug.WriteLine(connectionString);
                    process.StartInfo.Arguments = connectionString;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();

                    switch (process.ExitCode)
                    {
                        case 0:
                            Debug.WriteLine("Success!");
                            ConnectionStatus = "Connected!";
                            ConnectBtnContent = "Disconnect";
                            break;
                        case 691:
                            Debug.WriteLine("Invalid username or password!");
                            ConnectionStatus = "Invalid username or password!";
                            ConnectBtnContent = "Connect";
                            break;
                        default:
                            Debug.WriteLine($"Connection error: {process.ExitCode}");
                            ConnectionStatus = $"Connection error: {process.ExitCode}";
                            ConnectBtnContent = "Connect";
                            break;
                    }

                    // TODO: Disconnect when app is closed (using rasdial /d)
                });
            });
        }

        // Debug method: Print the server name that the user has selected in the list
        private void OnServerSelected()
        {
            if (SelectedServer != null)
            {
                Debug.WriteLine($"Selected Server: {SelectedServer.Country}");
                // Add your logic here to handle the selected server
            }
        }
    }
}
