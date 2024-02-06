using System.Diagnostics;
using System.Security;
using WinVPN.MVVM.Model;

namespace WinVPN.Core
{
    public class ConnectionService
    {
        private ConnectionStatus _connectionStatus;

        public ConnectionStatus Connect(ServerModel server, string username, SecureString password)
        {
            string passwordString = new System.Net.NetworkCredential(string.Empty, password).Password;

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            string connectionString = $"/c rasdial {server.Id} {username} {passwordString} /phonebook:./VPN/{server.Id}.pbk";
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
                    _connectionStatus = ConnectionStatus.Connected;
                    break;
                case 691:
                    Debug.WriteLine("Invalid username or password!");
                    _connectionStatus = ConnectionStatus.InvalidUsernameOrPassword;
                    break;
                case 868:
                    Debug.WriteLine("Host unreachable!");
                    _connectionStatus = ConnectionStatus.HostUnreachable;
                    break;
                default:
                    Debug.WriteLine($"Connection error: {process.ExitCode}");
                    _connectionStatus = ConnectionStatus.ConnectionError;
                    break;
            }

            return _connectionStatus;
        }

        public ConnectionStatus Disconnect()
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.Arguments = $"/c rasdial /d";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Debug.WriteLine("Disconnected!");
                _connectionStatus = ConnectionStatus.NotConnected;
            }
            else
            {
                Debug.WriteLine($"Disconnection error: {process.ExitCode}");
                _connectionStatus = ConnectionStatus.ConnectionError;
            }

            return _connectionStatus;
        }
    }
}
