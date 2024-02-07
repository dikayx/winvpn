using System.ComponentModel;

namespace WinVPN.Core
{
    public enum ConnectionStatus
    {
        [Description("Not connected.")]
        NOT_CONNECTED,
        [Description("Disconnecting...")]
        DISCONNECTING,
        [Description("Connecting...")]
        CONNECTING,
        [Description("Connected!")]
        CONNECTED,
        [Description("Invalid username or password.")]
        INVALID_CREDENTIALS,
        [Description("Connection error.")]
        CONNECTION_ERROR,
        [Description("Host unreachable.")]
        HOST_UNREACHABLE
    }

    public static class ConnectionStatusExtensions
    {
        /// <summary>
        /// This method returns the description of the enum value.
        /// The description is a user-friendly string that describes 
        /// the enum value.
        /// </summary>
        /// <param name="status">The enum value.</param>
        /// <returns>A string representation of the enum value.</returns>
        public static string GetDescription(this ConnectionStatus status)
        {
            var type = status.GetType();
            var memInfo = type.GetMember(status.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
