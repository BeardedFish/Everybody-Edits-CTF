// File Name:     CaptureTheFlagBot.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.GameMechanics;
using Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands;
using PlayerIOClient;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot
{
    public partial class CaptureTheFlagBot
    {
        /// <summary>
        /// States whether the bot is connected to Everybody Edits or not.
        /// </summary>
        public bool Connected => connection != null && connection.Connected;

        /// <summary>
        /// The connection object of the bot that allows communication with the Everybody Edits world it joined.
        /// </summary>
        private Connection connection;

        /// <summary>
        /// The connection information about the bot. This value is only set when <see cref="Connect(ConnectionInformation)"/> is invoked. If <see cref="Disconnect"/>
        /// is invoked, then this value becomes null.
        /// </summary>
        private ConnectionInformation connectionInformation;

        /// <summary>
        /// Constructor for creating an <see cref="CaptureTheFlagBot"/> object. The bot is able to host a Capture The Flag game in an Everybody Edits world.
        /// </summary>
        public CaptureTheFlagBot()
        {
            BotCommands = new Command[]
            {
                new AdminCommands(),
                new GameCommands(),
                new RegularCommands()
            };

            // Initialize game mechanics
            _ = new AntiCheat(this);
            _ = new AutoBalance(this);
            _ = new DailyBonus(this);
            _ = new DigSystem(this);
            _ = new FightSystem(this);
            FlagSystem = new FlagSystem(this);
            _ = new HealSystem(this);
            _ = new KillCredit(this);
            _ = new RespawnSystem(this);
            _ = new RoomEntrance(this);
            _ = new Shop(this);
            _ = new SmileyRoleNotification(this);
            _ = new TrapSystem(this);
            _ = new WarpPipe(this);

            CurrentGameRound = new GameRound();
            JoinedWorld = new WorldInformation();
        }

        /// <summary>
        /// Connects to an Everybody Edits world.
        /// </summary>
        /// <param name="connectionInformation">The bot connection information which contains the email, password, and world id.</param>
        /// <returns>
        /// Either null if the bot connected to Everybody Edits succesfully or a PlayerIOError object that contains the error on why the bot could not connect to Everybody
        /// Edits.
        /// </returns>
        public PlayerIOError Connect(ConnectionInformation connectionInformation)
        {
            try
            {
#pragma warning disable 612
                Client client = PlayerIO.QuickConnect.SimpleConnect(EverybodyEditsGameId, connectionInformation.Email, connectionInformation.Password, null);
#pragma warning restore 612

                connection = client.Multiplayer.CreateJoinRoom(connectionInformation.WorldId, "Everybodyedits" + client.BigDB.Load("config", "config")["version"], true, null, null);
                connection.OnDisconnect += OnDisconnect;
                connection.OnMessage += OnMessage;
                connection.Send(EverybodyEditsMessage.InitBegin);

                this.connectionInformation = connectionInformation;
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
        public void Disconnect()
        {
            SayChatMessage("Disconnecting...");
            SetWorldTitle($"{WorldTitle} [OFF]");

            connection?.Disconnect();
            connectionInformation = null;
        }

        /// <summary>
        /// Sends a message to the Everybody Edits server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void Send(string message)
        {
            connection?.Send(message);
        }

        /// <summary>
        /// Sends a message with parameters to the Everybody Edits server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="parameters">The parameters of the message.</param>
        public void Send(string message, params object[] parameters)
        {
            connection?.Send(message, parameters);
        }

        /// <summary>
        /// Sets the God mode status of the bot.
        /// </summary>
        /// <param name="turnOn">States whether God mode should be turned on or off.</param>
        public void SetGodMode(bool turnOn)
        {
            Send(EverybodyEditsMessage.GodModeToggled, turnOn);
        }

        /// <summary>
        /// Moves the bot to a specified location.
        /// </summary>
        /// <param name="loc">The location in the Everybody Edits world where the bot should move to.</param>
        public void Move(Point loc)
        {
            Send(EverybodyEditsMessage.PlayerMoved, loc.X * 16, loc.Y * 16, 0, 0, 0, 0, 0, 0, 0, false, false, 0);
        }

        /// <summary>
        /// Sends a chat message with a prefix to the Everybody Edits world via the bot. The prefix is defined in the <see cref="ChatMessagePrefix"/> variable. If the
        /// message is too long (greater than 140 characters, including the prefix), then it is sent in chunks.
        /// </summary>
        /// <param name="msg">The chat message to be sent.</param>
        public void SayChatMessage(string msg)
        {
            const int chunkSendDelayMs = 150;
            int maxChatMessageLength = 140 - ChatMessagePrefix.Length - 2;

            for (int i = 0; i < msg.Length; i += maxChatMessageLength)
            {
                Send(EverybodyEditsMessage.ChatMessage, $"{ChatMessagePrefix} {msg.Substring(i, Math.Min(maxChatMessageLength, msg.Length - i))}");

                Task.Delay(chunkSendDelayMs);
            }
        }

        /// <summary>
        /// Sends a private chat message to an Everybody Edits player.
        /// </summary>
        /// <param name="player">The player to send the private message to.</param>
        /// <param name="msg">The message to be sent.</param>
        public void SendPrivateMessage(Player player, string msg)
        {
            if (player.IsGuest) // Guests can't receive private messages
            {
                return;
            }

            Send(EverybodyEditsMessage.ChatMessage, "/pm " + player.Username + " " + msg);
        }
        
        /// <summary>
        /// Sets the title of the Everybody Edits world.
        /// </summary>
        public void SetWorldTitle(string title)
        {
            Send(EverybodyEditsMessage.SetTitle, title);
        }

        /// <summary>
        /// Kicks a player from the Everybody Edits world via their username.
        /// </summary>
        /// <param name="username">The username of the player to be kicked.</param>
        /// <param name="reason">The reason why the player should be kicked.</param>
        public void KickPlayer(string username, string reason)
        {
            Send(EverybodyEditsMessage.ChatMessage, $"/kick {username} {reason}");
        }

        /// <summary>
        /// Kills a player in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player to kill.</param>
        public void KillPlayer(Player player)
        {
            Send(EverybodyEditsMessage.ChatMessage, $"/kill {player.Username}");
        }

        /// <summary>
        /// Resets all players in the Everybody Edits world to the inital spawn locations of the world.
        /// </summary>
        public void ResetLevel()
        {
            Send(EverybodyEditsMessage.ChatMessage, "/resetall");
        }

        /// <summary>
        /// Resets a players properties and respawns them at a random respawn point in the Everybody Edits world. If a respawn point doesn't exit, they are respawned at the
        /// coordinate [1, 1].
        /// </summary>
        /// <param name="player">The player to be reset.</param>
        public void ResetPlayer(Player player)
        {
            Send(EverybodyEditsMessage.ChatMessage, $"/reset {player.Username}");
        }

        /// <summary>
        /// Places a block in the Everybody Edits world.
        /// </summary>
        /// <param name="layer">The layer of the block.</param>
        /// <param name="loc">The location of where the block will be placed.</param>
        /// <param name="block">The block to be placed.</param>
        public void PlaceBlock(BlockLayer layer, Point loc, Block block)
        {
            PlaceBlock(layer, loc, new MorphableBlock(block.Id, 0));
        }

        /// <summary>
        /// Places a block in the Everybody Edits world.
        /// </summary>
        /// <param name="layer">The layer of the block.</param>
        /// <param name="loc">The location of where the block will be placed.</param>
        /// <param name="morphableBlock">The morphable block to be placed.</param>
        public void PlaceBlock(BlockLayer layer, Point loc, MorphableBlock morphableBlock)
        {
            Send(EverybodyEditsMessage.PlaceBlock, (int)layer, loc.X, loc.Y, morphableBlock.Id, morphableBlock.MorphId);
        }

        /// <summary>
        /// Removes fly effect, jump effect, and speed effect from a player.
        /// </summary>
        /// <param name="player">The player to remove the effect from.</param>
        public void RemoveEffects(Player player)
        {
            string[] effects = { "fly", "jump", "speed" };

            foreach (string effect in effects)
            {
                Send(EverybodyEditsMessage.ChatMessage, $"/removeeffect {player.Username} {effect}");
            }
        }

        /// <summary>
        /// Teleports a player to a specified location in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player to teleport.</param>
        /// <param name="x">The x location to teleport the player to.</param>
        /// <param name="y">The y location to teleport the player to.</param>
        public void TeleportPlayer(Player player, int x, int y)
        {
            Send(EverybodyEditsMessage.ChatMessage, $"/teleport {player.Username} {x} {y}");
        }

        /// <summary>
        /// Sets the force fly state of a specified player in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player to be modified.</param>
        /// <param name="forceFly">States whether the player should force fly (true) or not (false).</param>
        public void SetForceFly(Player player, bool forceFly)
        {
            Send(EverybodyEditsMessage.ChatMessage, $"/forcefly {player.Username} {forceFly}");
        }
    }
}