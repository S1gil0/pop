using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Rust;

namespace Oxide.Plugins
{
    [Info("Pop", "Sigilo", "1.0.0")]
    [Description("Displays the number of connected and connecting players.")]

    public class Pop : RustPlugin
    {
        private void Init()
        {
            permission.RegisterPermission("pop.use", this);
        }

        [ChatCommand("pop")]
        private void PopCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, "pop.use"))
            {
                SendReply(player, "You don't have permission to use this command.");
                return;
            }

            int connectedPlayers = BasePlayer.activePlayerList.Count;
            int joiningPlayers = Network.Net.sv.connections.Count - connectedPlayers;

            string message = string.Format("Players Online: {0}/{1} | Players Joining: {2}", connectedPlayers, ConVar.Server.maxplayers, joiningPlayers);
            SendReply(player, message);
        }
    }
}