// File Name:     CaptureTheFlagBot.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.GameMechanics;
using Everybody_Edits_CTF.Core.GameMechanics.Enums;
using Everybody_Edits_CTF.Enums;
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Everybody_Edits_CTF.Core.Bot
{
    public static class CaptureTheFlagBot
    {
        public static bool Connected
        {
            get
            {
                return connection != null && connection.Connected;
            }
        }

        public static Dictionary<int, Player> PlayersInWorld
        {
            get
            {
                return botEventHandler?.PlayersInWorld;
            }
        }

        private static readonly int MaxChatMessageLength = 140 - GameSettings.ChatMessagePrefix.Length - 2;

        private static BotEventHandler botEventHandler;
        private static Client client;
        private static Connection connection;

        /// <summary>
        /// Connects the bot to Everybody Edits.
        /// </summary>
        /// <returns>
        /// Either null if the bot connected to Everybody Edits succesfully or a PlayerIOError object that contains the error on why the bot could not connect to Everybody Edits.
        /// </returns>
        public static PlayerIOError Connect()
        {
            try
            {
#pragma warning disable 612
                client = PlayerIO.QuickConnect.SimpleConnect(GameSettings.EverybodyEditsGameId, GameSettings.BotEmail, GameSettings.BotPassword, null);
#pragma warning restore 612

                connection = client.Multiplayer.CreateJoinRoom(GameSettings.WorldId, "Everybodyedits" + client.BigDB.Load("config", "config")["version"], true, null, null);
                connection.Send(EverybodyEditsMessage.InitBegin);

                botEventHandler = new BotEventHandler(connection);
            }
            catch (PlayerIOError error)
            {
                return error;
            }

            return null;
        }

        /// <summary>
        /// Disconnects the bot from Everybody Edits.
        /// </summary>
        public static void Disconnect()
        {
            SendChatMessage("Disconnecting...");
            SetWorldTitle(false);

            connection?.Disconnect();
        }

        public static void SetGodMode(bool turnOn)
        {
            connection?.Send(EverybodyEditsMessage.GodModeToggled, turnOn);
        }

        public static void Move(Point loc)
        {
            connection?.Send(EverybodyEditsMessage.PlayerMoved, loc.X * 16, loc.Y * 16, 0, 0, 0, 0, 0, 0, 0, false, false, 0);
        }

        public static void Send(string command)
        {
            connection?.Send(command);
        }

        /// <summary>
        /// Sends a chat message to the Everybody Edits world via the bot.
        /// </summary>
        /// <param name="msg">The message to be sent.</param>
        public static void SendChatMessage(string msg)
        {
            string msgChunk;
            for (int i = 0; i < msg.Length; i += MaxChatMessageLength)
            {
                msgChunk = msg.Substring(i, Math.Min(MaxChatMessageLength, msg.Length - i));

                connection?.Send(EverybodyEditsMessage.ChatMessage, GameSettings.ChatMessagePrefix + " " + msgChunk);

                Thread.Sleep(150);
            }
        }

        /// <summary>
        /// Sends a private chat message to an Everybody Edits player via the bot.
        /// </summary>
        /// <param name="player">The player to send the private message to.</param>
        /// <param name="msg">The message to be sent.</param>
        public static void SendPrivateMessage(Player player, string msg)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, "/pm " + player.Username + " " + msg);
        }

        public static void SetWorldTitle(bool botOn)
        {
            connection?.Send("name", $"CTF Bot [{(botOn ? "ON" : "OFF")}]");
        }

        public static void KickPlayer(string username, string reason)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, $"/kick {username} {reason}");
        }

        public static void KillPlayer(Player playerToKill, Player playerKiller, DeathReason reason)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, "/kill " + playerToKill.Username);

            if (reason == DeathReason.Hazard || reason == DeathReason.Suicide)
            {
                string msg = reason == DeathReason.Hazard ? "You were killed by a hazard." : "You comitted suicide.";
                SendPrivateMessage(playerToKill, msg);
            }
            
            if (reason == DeathReason.Player)
            {
                SendPrivateMessage(playerToKill, $"You were killed by player {playerKiller.Username}!");
                SendPrivateMessage(playerKiller, $"You killed player {playerToKill.Username}!");

                CaptureTheFlag.IncreaseGameFund(GameFundIncreaseReason.PlayerKilledEnemy);
            }

            playerToKill.RestoreHealth();
            playerToKill.LastAttacker = null;
        }

        public static void LoadLevel()
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, "/loadlevel");
        }

        public static void PlaceBlock(int layer, int x, int y, int blockId)
        {
            PlaceBlock(layer, x, y, blockId, 0);
        }

        public static void PlaceBlock(int layer, int x, int y, int blockId, int morphId)
        {
            connection?.Send(EverybodyEditsMessage.PlaceBlock, layer, x, y, blockId, morphId);
        }

        public static void TeleportPlayer(Player player, int x, int y)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, $"/teleport {player.Username} {x} {y}");
        }
    }
}