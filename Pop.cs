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
    [Info("Pop", "Sigilo", "1.7.2")]
    [Description("Displays the number of connected, connecting, and queued players.")]

    public class Pop : CovalencePlugin
    {
        private string command;
        private string broadcastCommand;
        private string onlineColor;
        private string joiningColor;
        private string queuedColor;
        private int broadcastDelay;
        private DateTime lastBroadcast;
        private bool enablePersonalCommand;
        private bool enableGlobalCommand;

        private void Init()
        {
            permission.RegisterPermission("pop.use", this);
            LoadConfig();
            if (enablePersonalCommand && !string.IsNullOrEmpty(command))
            {
                AddCovalenceCommand(command, "PopCommand");
            }
            if (enableGlobalCommand && !string.IsNullOrEmpty(broadcastCommand))
            {
                AddCovalenceCommand(broadcastCommand, "BroadcastPopCommand");
            }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                command = Config.Get<string>("Command") ?? "pop";
                broadcastCommand = Config.Get<string>("BroadcastCommand") ?? "!pop";
                onlineColor = Config.Get<string>("OnlineColor") ?? "#ff686b";
                joiningColor = Config.Get<string>("JoiningColor") ?? "#ff686b";
                queuedColor = Config.Get<string>("QueuedColor") ?? "#ff686b";
                broadcastDelay = Config.Get<int>("BroadcastDelay");
                enablePersonalCommand = Config.Get<bool>("EnablePersonalCommand");
                enableGlobalCommand = Config.Get<bool>("EnableGlobalCommand");
            }
            catch (Exception ex)
            {
                PrintError($"Error loading config: {ex.Message}");
                LoadDefaultConfig();
            }
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating new configuration file");
            Config.Clear();
            Config["Command"] = "pop";
            Config["BroadcastCommand"] = "!pop";
            Config["OnlineColor"] = "#ff686b";
            Config["JoiningColor"] = "#ff686b";
            Config["QueuedColor"] = "#ff686b";
            Config["BroadcastDelay"] = 10;
            Config["EnablePersonalCommand"] = true;
            Config["EnableGlobalCommand"] = true;
            SaveConfig();
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
            int joiningPlayers = ServerMgr.Instance.connectionQueue.Joining;
            int playersInQueue = GetQueuedPlayersCount();

            string message = string.Format("Players Online: {0} | Joining: {1} | Queued: {2}",
                string.Format("<color={0}>{1}</color>", onlineColor, connectedPlayers),
                string.Format("<color={0}>{1}</color>", joiningColor, joiningPlayers),
                string.Format("<color={0}>{1}</color>", queuedColor, playersInQueue));

            player.Reply(message);
        }

        private void BroadcastPopCommand(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, "pop.use"))
            {
                player.Reply(lang.GetMessage("NoPermission", this, player.Id));
                return;
            }

            if (DateTime.Now < lastBroadcast.AddMinutes(broadcastDelay))
            {
                string tooSoonMessage = string.Format(lang.GetMessage("TooSoon", this, player.Id), broadcastDelay);
                player.Reply(tooSoonMessage);
                return;
            }

            lastBroadcast = DateTime.Now;

            int connectedPlayers = BasePlayer.activePlayerList.Count;
            int joiningPlayers = ServerMgr.Instance.connectionQueue.Joining;
            int playersInQueue = GetQueuedPlayersCount();

            string message = string.Format("Players Online: {0} | Joining: {1} | Queued: {2}",
                string.Format("<color={0}>{1}</color>", onlineColor, connectedPlayers),
                string.Format("<color={0}>{1}</color>", joiningColor, joiningPlayers),
                string.Format("<color={0}>{1}</color>", queuedColor, playersInQueue));

            server.Broadcast(message);
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"NoPermission", "You don't have permission to use this command."},
                {"TooSoon", "You must wait {0} minutes between broadcasts (try /pop)."},
                {"PopMessage", "Players Online: {0}/{1} | Joining: {2} | Queued: {3}"}
            }, this);
        }

        void OnUserChat(IPlayer player, string message)
        {
            if (message == "!pop")
            {
                BroadcastPopCommand(player, message, null);
            }
        }
    }
}