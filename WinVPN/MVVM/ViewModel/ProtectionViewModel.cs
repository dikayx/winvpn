using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Windows;
using WinVPN.Core;
using WinVPN.MVVM.Model;

namespace WinVPN.MVVM.ViewModel
{
    internal class ProtectionViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<ServerModel> Servers { get; set; }
        private ServerBuilder _serverBuilder;
        private ConnectionService _connectionService;

        private ConnectionStatus _vpnConnectionStatus;
        public string VpnConnectionStatus
        {
            get
            {
                return _vpnConnectionStatus switch
                {
                    ConnectionStatus.Connected => "Connected",
                    ConnectionStatus.NotConnected => "Not Connected",
                    ConnectionStatus.InvalidUsernameOrPassword => "Invalid Username Or Password",
                    ConnectionStatus.ConnectionError => $"Connection Error",
                    _ => "Not Connected",
                };
            }
            set
            {
                _vpnConnectionStatus = value switch
                {
                    "Connected" => ConnectionStatus.Connected,
                    "Not Connected" => ConnectionStatus.NotConnected,
                    "InvalidUsernameOrPassword" => ConnectionStatus.InvalidUsernameOrPassword,
                    "ConnectionError" => ConnectionStatus.ConnectionError,
                    _ => ConnectionStatus.NotConnected,
                };
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
            // Create a new ServerBuilder
            _serverBuilder = new ServerBuilder();
            Servers = _serverBuilder.GetServers();

            // Create a new ConnectionService
            _connectionService = new ConnectionService();

            ConnectCommand = new RelayCommand(o =>
            {
                Task.Run(() =>
                {
                    if (VpnConnectionStatus == "Connected")
                    {
                        VpnConnectionStatus = "Disconnecting...";
                        ConnectBtnContent = "Disconnecting...";
                        _connectionService.Disconnect();
                        VpnConnectionStatus = "Not Connected";
                        ConnectBtnContent = "Connect";
                        return;
                    }

                    if (SelectedServer == null)
                    {
                        MessageBox.Show("Please select a server!");
                        return;
                    }

                    //VpnConnectionStatus = "Connecting...";
                    ConnectBtnContent = "Connecting...";

                    ConnectionStatus status = _connectionService.Connect(SelectedServer, Username, Password);
                    VpnConnectionStatus = status.ToString();

                    // Update the button content
                    if (status == ConnectionStatus.Connected)
                    {
                        ConnectBtnContent = "Disconnect";
                    }
                    else
                    {
                        ConnectBtnContent = "Connect";
                    }
                });
            });
        }

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
