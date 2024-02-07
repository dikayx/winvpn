using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;
using System.Windows;
using WinVPN.Core;
using WinVPN.MVVM.Model;

namespace WinVPN.MVVM.ViewModel
{
    internal class ProtectionViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<ServerModel> Servers { get; set; }
        public RelayCommand ConnectCommand { get; set; }

        private ServerBuilder _serverBuilder;
        private ConnectionService _connectionService;

        private ServerModel _selectedServer;
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                _selectedServer = value;
                OnPropertyChanged();
            }
        }

        private ConnectionStatus _status;
        public ConnectionStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionStatusDescription));
            }
        }
        // Additional property for a user-friendly status description
        public string ConnectionStatusDescription => Status.GetDescription();

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

        private string _connectButtonContent = "Connect";
        public string ConnectButtonContent
        {
            get => _connectButtonContent;
            set
            {
                if (_connectButtonContent != value)
                {
                    _connectButtonContent = value;
                    OnPropertyChanged(nameof(ConnectButtonContent));
                }
            }
        }

        public ProtectionViewModel()
        {
            _serverBuilder = new ServerBuilder();
            Servers = _serverBuilder.GetServers();
            _connectionService = new ConnectionService();

            ConnectCommand = new RelayCommand(o =>
            {
                Task.Run(() =>
                {
                    if (Status == ConnectionStatus.CONNECTED)
                    {
                        Status = ConnectionStatus.DISCONNECTING;
                        ConnectButtonContent = "Disconnecting...";
                        _connectionService.Disconnect();
                        Status = ConnectionStatus.NOT_CONNECTED;
                        ConnectButtonContent = "Connect";
                        return;
                    }

                    if (SelectedServer == null)
                    {
                        MessageBox.Show("Please select a server!");
                        return;
                    }

                    ConnectButtonContent = "Connecting...";

                    Status = _connectionService.Connect(SelectedServer, Username, Password);
                    if (Status == ConnectionStatus.CONNECTED)
                    {
                        ConnectButtonContent = "Disconnect";
                    }
                    else
                    {
                        ConnectButtonContent = "Connect";
                    }
                });
            });
        }
    }
}
