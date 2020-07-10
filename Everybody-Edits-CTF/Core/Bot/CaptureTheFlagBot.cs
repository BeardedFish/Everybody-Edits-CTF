// File Name:     CaptureTheFlagBot.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.Settings;
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Everybody_Edits_CTF.Core.Bot
{
    public static class CaptureTheFlagBot
    {
        /// <summary>
        /// States whether the bot is connected to Everybody Edits or not.
        /// </summary>
        public static bool Connected
        {
            get => connection != null && connection.Connected;
        }

        /// <summary>
        /// All the players currently connected in the Everybody Edits world. The key (int) defines the player's id, while the value (Player) defines the object that contains
        /// data about the player.
        /// </summary>
        public static Dictionary<int, Player> PlayersInWorld
        {
            get => botEventHandler?.PlayersInWorld;
        }

        /// <summary>
        /// The max length of a chat message Everybody Edits allows to be sent.
        /// </summary>
        private static readonly int MaxChatMessageLength = 140 - BotSettings.ChatMessagePrefix.Length - 2;

        /// <summary>
        /// The handler for all the message events that the bot receives.
        /// </summary>
        private static BotEventHandler botEventHandler;

        /// <summary>
        /// The connection object of the bot that allows communication with the Everybody Edits world it joined.
        /// </summary>
        private static Connection connection;

        /// <summary>
        /// Connects the bot to the Everybody Edits world defined in <see cref="BotSettings.WorldId"/>.
        /// </summary>
        /// <returns>
        /// Either null if the bot connected to Everybody Edits succesfully or a PlayerIOError object that contains the error on why the bot could not connect to Everybody Edits.
        /// </returns>
        public static PlayerIOError Connect()
        {
            try
            {
#pragma warning disable 612
                Client client = PlayerIO.QuickConnect.SimpleConnect(BotSettings.EverybodyEditsGameId, BotSettings.Email, BotSettings.Password, null);
#pragma warning restore 612

                connection = client.Multiplayer.CreateJoinRoom(BotSettings.WorldId, "Everybodyedits" + client.BigDB.Load("config", "config")["version"], true, null, null);
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
            SetWorldTitle($"{BotSettings.WorldTitle} [OFF]");

            connection?.Disconnect();
        }

        /// <summary>
        /// Sets the God mode status of the bot.
        /// </summary>
        /// <param name="turnOn">States whether God mode should be turned on or off.</param>
        public static void SetGodMode(bool turnOn)
        {
            connection?.Send(EverybodyEditsMessage.GodModeToggled, turnOn);
        }

        /// <summary>
        /// Moves the bot to a specified location.
        /// </summary>
        /// <param name="loc">The location in the Everybody Edits world where the bot should move to.</param>
        public static void Move(Point loc)
        {
            connection?.Send(EverybodyEditsMessage.PlayerMoved, loc.X * 16, loc.Y * 16, 0, 0, 0, 0, 0, 0, 0, false, false, 0);
        }

        /// <summary>
        /// Sends a command to the Everybody Edits world.
        /// </summary>
        /// <param name="command">The command to be sent.</param>
        public static void Send(string command)
        {
            connection?.Send(command);
        }

        /// <summary>
        /// Sends a chat message with a prefix to the Everybody Edits world via the bot. The prefix is defined in <see cref="BotSettings.ChatMessagePrefix"/>. If the message length
        /// is greater than <see cref="MaxChatMessageLength"/>, then the message is split into chunks where each message chunk is sent separately.
        /// </summary>
        /// <param name="msg">The chat message to be sent.</param>
        public static void SendChatMessage(string msg)
        {
            string msgChunk;
            for (int i = 0; i < msg.Length; i += MaxChatMessageLength)
            {
                msgChunk = msg.Substring(i, Math.Min(MaxChatMessageLength, msg.Length - i));

                connection?.Send(EverybodyEditsMessage.ChatMessage, BotSettings.ChatMessagePrefix + " " + msgChunk);

                Thread.Sleep(150);
            }
        }

        /// <summary>
        /// Sends a private chat message to an Everybody Edits player.
        /// </summary>
        /// <param name="player">The player to send the private message to.</param>
        /// <param name="msg">The message to be sent.</param>
        public static void SendPrivateMessage(Player player, string msg)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, "/pm " + player.Username + " " + msg);
        }
        
        /// <summary>
        /// Sets the title of the Everybody Edits world.
        /// </summary>
        public static void SetWorldTitle(string title)
        {
            connection?.Send(EverybodyEditsMessage.SetTitle, title);
        }

        /// <summary>
        /// Kicks a player from the Everybody Edits world via their username.
        /// </summary>
        /// <param name="username">The username of the player to be kicked.</param>
        /// <param name="reason">The reason why the player should be kicked.</param>
        public static void KickPlayer(string username, string reason)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, $"/kick {username} {reason}");
        }

        /// <summary>
        /// Kills a player in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player to kill.</param>
        public static void KillPlayer(Player player)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, $"/kill {player.Username}");
        }

        /// <summary>
        /// Resets all players in the Everybody Edits world to the inital spawn locations of the world.
        /// </summary>
        public static void ResetLevel()
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, "/resetall");
        }

        /// <summary>
        /// Places a block in the Everybody Edits world.
        /// </summary>
        /// <param name="layer">The layer of the block.</param>
        /// <param name="loc">The location of where the block will be placed.</param>
        /// <param name="blockId">The id of the block to be placed.</param>
        public static void PlaceBlock(BlockLayer layer, Point loc, int blockId)
        {
            PlaceBlock(layer, loc, blockId, 0);
        }

        /// <summary>
        /// Places a block in the Everybody Edits world.
        /// </summary>
        /// <param name="layer">The layer of the block.</param>
        /// <param name="loc">The location of where the block will be placed.</param>
        /// <param name="blockId">The id of the block to be placed.</param>
        /// <param name="morphId">The morph id of the block to be placed.</param>
        public static void PlaceBlock(BlockLayer layer, Point loc, int blockId, int morphId)
        {
            connection?.Send(EverybodyEditsMessage.PlaceBlock, (int)layer, loc.X, loc.Y, blockId, morphId);
        }

        /// <summary>
        /// Removes fly effect, jump effect, and speed effect from a player.
        /// </summary>
        /// <param name="player">The player to remove the effect from.</param>
        public static void RemoveEffects(Player player)
        {
            string[] effects = { "fly", "jump", "speed" };

            foreach (string effect in effects)
            {
                connection?.Send(EverybodyEditsMessage.ChatMessage, $"/removeeffect {player.Username} {effect}");
            }
        }

        /// <summary>
        /// Teleports a player to a specified location in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player to teleport.</param>
        /// <param name="x">The x location to teleport the player to.</param>
        /// <param name="y">The y location to teleport the player to.</param>
        public static void TeleportPlayer(Player player, int x, int y)
        {
            connection?.Send(EverybodyEditsMessage.ChatMessage, $"/teleport {player.Username} {x} {y}");
        }
    }
}