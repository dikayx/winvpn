using System.Diagnostics;
using System.Security;
using WinVPN.MVVM.Model;

namespace WinVPN.Core
{
    public class ConnectionService
    {
        private ConnectionStatus _connectionStatus;

        public ConnectionStatus Connect(ServerModel server, string username, SecureString? password)
        {
            string passwordString = new System.Net.NetworkCredential(string.Empty, password).Password;

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            string connectionString = $"/c rasdial {server.Id} {username} {passwordString} /phonebook:./VPN/{server.Id}.pbk";
            process.StartInfo.Arguments = connectionString;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForExit();

            switch (process.ExitCode)
            {
                case 0:
                    FileLogger.Log($"Successfully connected to {server.Server}");
                    _connectionStatus = ConnectionStatus.CONNECTED;
                    break;
                case 691:
                    FileLogger.Log($"Invalid credentials for {server.Server}");
                    _connectionStatus = ConnectionStatus.INVALID_CREDENTIALS;
                    break;
                case 868:
                    FileLogger.Log("The remote server is not reachable");
                    _connectionStatus = ConnectionStatus.HOST_UNREACHABLE;
                    break;
                default:
                    FileLogger.Log($"Error while connecting to {server.Server}. Code: {process.ExitCode}");
                    _connectionStatus = ConnectionStatus.CONNECTION_ERROR;
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
                FileLogger.Log("Successfully disconnected");
                _connectionStatus = ConnectionStatus.NOT_CONNECTED;
            }
            else
            {
                FileLogger.Log($"Error while disconnecting. Code: {process.ExitCode}");
                _connectionStatus = ConnectionStatus.CONNECTION_ERROR;
            }

            return _connectionStatus;
        }
    }
}
