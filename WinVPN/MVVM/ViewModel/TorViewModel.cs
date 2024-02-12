using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using WinVPN.Core;

namespace WinVPN.MVVM.ViewModel
{
    internal class TorViewModel : ObservableObject, INotifyPropertyChanged
    {
        // Private fields
        private string? _torExecutablePath;

        // Bindable properties
        public RelayCommand? LaunchCommand { get; set; }
        private string _launchTorButtonContent = "Launch Tor Browser";
        public string LaunchTorButton
        {
            get => _launchTorButtonContent;
            set
            {
                if (_launchTorButtonContent != value)
                {
                    _launchTorButtonContent = value;
                    OnPropertyChanged(nameof(LaunchTorButton));
                }
            }
        }

        private bool _isTorLaunchButtonEnabled = false;
        public bool IsTorLaunchButtonEnabled
        {
            get => _isTorLaunchButtonEnabled;
            set
            {
                if (_isTorLaunchButtonEnabled != value)
                {
                    _isTorLaunchButtonEnabled = value;
                    OnPropertyChanged(nameof(IsTorLaunchButtonEnabled));
                }
            }
        }

        private string _torStatus = "Couldn't detect Tor installation...";
        public string TorStatus
        {
            get => _torStatus;
            set
            {
                if (_torStatus != value)
                {
                    _torStatus = value;
                    OnPropertyChanged(nameof(TorStatus));
                }
            }
        }

        private string _torLogoSource = "https://www.torproject.org/static/images/tor-logo/Black.png";
        public string TorLogoSource
        {
            get
            {
                if (IsTorLaunchButtonEnabled)
                {
                    return "https://www.torproject.org/static/images/tor-logo/Color.png";
                }
                return _torLogoSource;
            }
            set
            {
                if (_torLogoSource != value)
                {
                    _torLogoSource = value;
                    OnPropertyChanged(nameof(TorLogoSource));
                }
            }
        }

        public TorViewModel()
        {
            if (IsTorBrowserInstalled())
            {
                IsTorLaunchButtonEnabled = true;
                TorStatus = "Tor Browser is installed!";
                LaunchCommand = new RelayCommand(o =>
                {
                    Task.Run(() =>
                    {
                        FileLogger.Log("Launching Tor Browser");
                        LaunchTorBrowser();
                    });
                });
            }
            else
            {
                IsTorLaunchButtonEnabled = false;
                TorStatus = "Tor Browser is not installed!";
            }
        }

        private bool IsTorBrowserInstalled()
        {
            // Check if Tor Browser is installed on the desktop
            string desktopTorBrowserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Tor Browser");
            if (Directory.Exists(desktopTorBrowserPath))
            {
                string desktopTorBrowserExecutablePath = Path.Combine(desktopTorBrowserPath, "Browser", "firefox.exe");
                if (File.Exists(desktopTorBrowserExecutablePath))
                {
                    _torExecutablePath = desktopTorBrowserExecutablePath;
                    return true;
                }
            }

            // Check if Tor Browser is installed in the %APPDATA%\Roaming directory
            string appDataTorBrowserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tor Browser");
            if (Directory.Exists(appDataTorBrowserPath))
            {
                string appDataTorBrowserExecutablePath = Path.Combine(appDataTorBrowserPath, "Browser", "firefox.exe");
                if (File.Exists(appDataTorBrowserExecutablePath))
                {
                    _torExecutablePath = appDataTorBrowserExecutablePath;
                    return true;
                }
            }

            return false;
        }

        private void LaunchTorBrowser()
        {
            string torExecutablePath = _torExecutablePath ?? throw new InvalidOperationException("Tor Browser executable path is null!");

            if (File.Exists(torExecutablePath))
            {
                var process = new Process();
                process.StartInfo.FileName = torExecutablePath;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(torExecutablePath);

                FileLogger.Log($"Trying to launch Tor Browser from {torExecutablePath}");
                try
                {
                    process.Start();
                    FileLogger.Log("Tor Browser launched successfully!");
                }
                catch (Exception ex)
                {
                    FileLogger.Log($"Error launching Tor Browser: {ex.Message}");
                }
            }
            else
            {
                FileLogger.Log("Tor Browser executable not found!");
            }
        }
    }
}
