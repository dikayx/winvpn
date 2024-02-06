namespace WinVPN.Core
{
    public enum ConnectionStatus
    {
        NotConnected,
        Connecting,
        Connected,
        InvalidUsernameOrPassword,
        ConnectionError,
        HostUnreachable
    }
}
