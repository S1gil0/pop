using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Rust;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Pop", "Sigilo", "1.1.0")]
    [Description("Displays the number of connected and connecting players.")]

    public class Pop : CovalencePlugin
    {
        private string command;

        private void Init()
        {
            permission.RegisterPermission("pop.use", this);
            LoadConfig();
            if (!string.IsNullOrEmpty(command))
            {
                AddCovalenceCommand(command, "PopCommand");
            }
        }

        protected override void LoadDefaultConfig()
        {
            Config["Command"] = "pop";
        }

        private void LoadConfig()
        {
            try
            {
                command = Config.Get<string>("Command");
            }
            catch
            {
                LoadDefaultConfig();
            }

            SaveConfig();
        }

        private void SaveConfig()
        {
            Config["Command"] = command;
            Config.Save();
        }

        private void PopCommand(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, "pop.use"))
            {
                player.Reply(lang.GetMessage("NoPermission", this, player.Id));
                return;
            }

            int connectedPlayers = BasePlayer.activePlayerList.Count;
            int joiningPlayers = Network.Net.sv.connections.Count - connectedPlayers;

            string message = string.Format(lang.GetMessage("PopMessage", this, player.Id), connectedPlayers, ConVar.Server.maxplayers, joiningPlayers);
            player.Reply(message);
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"NoPermission", "You don't have permission to use this command."},
                {"PopMessage", "Players Online: {0}/{1} | Players Joining: {2}"}
            }, this);
        }
    }
}