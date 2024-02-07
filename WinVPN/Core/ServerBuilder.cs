using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using WinVPN.MVVM.Model;

namespace WinVPN.Core
{
    public class ServerBuilder
    {
        private ObservableCollection<ServerModel> Servers { get; set; }

        public ServerBuilder()
        {
            var username = "vpnbook";
            var suffix = ".vpnbook.com";
            var ids = new List<string> { "US1", "US2", "CA196", "DE220", "FR231", "PL140", "UK68" };

            var countries = new Dictionary<string, string> // Key: Country, Value: Flag URL
            {
                { "USA1", "https://i.imgur.com/tX2FzGr.png" },
                { "USA2", "https://i.imgur.com/tX2FzGr.png" },
                { "Canada", "https://i.imgur.com/VIxuFmK.png" },
                { "Germany", "https://i.imgur.com/l66r6qD.png" },
                { "France", "https://i.imgur.com/XohHXyD.png" },
                { "Poland", "https://i.imgur.com/k6ie3Ra.png" },
                { "UK", "https://i.imgur.com/QW2YV9c.png" },
            };

            Servers = new ObservableCollection<ServerModel>();

            foreach (var country in countries)
            {
                var id = ids[countries.Keys.ToList().IndexOf(country.Key)];
                // Create a new server
                Servers.Add(new ServerModel
                {
                    Id = id,
                    Username = username,
                    Server = $"{id}{suffix}",
                    Country = country.Key,
                    Flag = country.Value,
                });

                // Create a .pbk file for each server
                CreatePbkFile(Servers.Last());
            }
        }

        private void CreatePbkFile(ServerModel server)
        {
            var filename = server.Id; // "US1"
            var address = server.Server; // "US1.vpnbook.com"

            Debug.WriteLine(Directory.GetCurrentDirectory());

            var folderpath = Path.Combine(Directory.GetCurrentDirectory(), "VPN");
            var pbkpath = Path.Combine(folderpath, $"{filename}.pbk");

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            if (File.Exists(pbkpath))
            {
                Debug.WriteLine("File already exists, skipping...");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"[{filename}]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            sb.AppendLine("Device=WAN Miniport (IKEv2)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine($"PhoneNumber={address}");

            File.WriteAllText(pbkpath, sb.ToString());
        }

        public ObservableCollection<ServerModel> GetServers()
        {
            return Servers;
        }
    }
}
