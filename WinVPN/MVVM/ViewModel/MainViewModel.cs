using System.Windows;
using WinVPN.Core;
using WinVPN.MVVM.View;

namespace WinVPN.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        // Commands
        public RelayCommand MoveWindowCommand { get; set; }
        public RelayCommand ShutdownWindowCommand { get; set; }
        public RelayCommand MaximizeWindowCommand { get; set; }
        public RelayCommand MinimizeWindowCommand { get; set; }
        public RelayCommand ShowProtectionView { get; set; }
        public RelayCommand ShowSettingsView { get; set; }
        public RelayCommand ShowTorView { get; set; }

        // Services
        private ConnectionService _connectionService;


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ProtectionViewModel ProtectionVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public TorViewModel TorVM { get; set; }


        public MainViewModel()
        {
            _connectionService = new ConnectionService();

            ProtectionVM = new ProtectionViewModel();
            SettingsVM = new SettingsViewModel();
            TorVM = new TorViewModel();
            CurrentView = ProtectionVM;

            Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            MoveWindowCommand = new RelayCommand(o => Application.Current.MainWindow.DragMove());
            ShutdownWindowCommand = new RelayCommand(o =>
            {
                var status = _connectionService.Disconnect();
                if (status == ConnectionStatus.CONNECTED)
                {
                    MessageBox.Show("Could not close the existing connection.\nTry to disconnect manually using 'rasidial /d'", 
                        "WinVPN", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Application.Current.Shutdown();
            });
            // TODO: May need rework
            //MaximizeWindowCommand = new RelayCommand(o => Application.Current.MainWindow.WindowState ^= WindowState.Maximized);
            //MinimizeWindowCommand = new RelayCommand(o => Application.Current.MainWindow.WindowState = WindowState.Minimized);
            MaximizeWindowCommand = new RelayCommand(o =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                }
            });
            MinimizeWindowCommand = new RelayCommand(o => Application.Current.MainWindow.WindowState = WindowState.Minimized);

            ShowProtectionView = new RelayCommand(o => CurrentView = ProtectionVM);
            ShowSettingsView = new RelayCommand(o => CurrentView = SettingsVM);
            ShowTorView = new RelayCommand(o => CurrentView = TorVM);
        }
    }
}
