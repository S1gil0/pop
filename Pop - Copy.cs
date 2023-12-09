using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries.Covalence;
using Rust;

namespace Oxide.Plugins
{
    [Info("Pop", "Sigilo", "1.7.1")]
    [Description("Displays the number of connected, connecting, and queued players.")]

    public class Pop : CovalencePlugin
    {
        private string command;

        private void Init()
        {
            permission.RegisterPermission("pop.use", this);
            LoadConfig();
            if (string.IsNullOrEmpty(command))
            {
                PrintWarning("Command is null or empty.");
            }
            else
            {
                AddCovalenceCommand(command, "PopCommand");
            }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                command = Config.Get<string>("Command");
            }
            catch (Exception ex)
            {
                PrintError($"Error loading config: {ex.Message}");
                throw;
            }
        }

        protected override void SaveConfig()
        {
            Config["Command"] = command;
            Config.Save();
        }

        private int GetQueuedPlayersCount()
        {
            return ServerMgr.Instance.connectionQueue.Queued;
        }

        private void PopCommand(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, "pop.use"))
            {
                player.Reply(lang.GetMessage("NoPermission", this, player.Id));
                return;
            }

            int connectedPlayers = BasePlayer.activePlayerList.Count;
            int joiningPlayers = ServerMgr.Instance.connectionQueue.Joining - GetQueuedPlayersCount();
            int playersInQueue = GetQueuedPlayersCount();

            string message = lang.GetMessage("PopMessage", this, player.Id);
            message = string.Format(CultureInfo.InvariantCulture, message, connectedPlayers, ConVar.Server.maxplayers, joiningPlayers, playersInQueue);
            player.Reply(message);
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"NoPermission", "You don't have permission to use this command."},
                {"PopMessage", "Players Online: {0}/{1} | Joining: {2} | Queued: {3}"}
            }, this);
        }
    }
}