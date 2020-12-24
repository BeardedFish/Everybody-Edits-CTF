// File Name:     CaptureTheFlagBot.EventHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, August 14, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Deserializer;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;
using Everybody_Edits_CTF.Core.Bot.EventArgs;
using Everybody_Edits_CTF.Core.Bot.GameMechanics;
using Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot
{
    public partial class CaptureTheFlagBot
    {
        /// <summary>
        /// States whether this bot has received the <see cref="EverybodyEditsMessage.InitEnd"/> message or not.
        /// </summary>
        public bool FinishedInit { get; private set; }

        /// <summary>
        /// The object that contains information about the current Capture The Flag game round.
        /// </summary>
        public GameRound CurrentGameRound { get; private set; }

        /// <summary>
        /// The array of command objects. This array will always have a length of three.
        /// 
        /// Index Map:
        /// 0 - Admin commands
        /// 1 - Game commands
        /// 2 - Regular commands
        /// </summary>
        public Command[] BotCommands;

        /// <summary>
        /// The flag system for the Capture The Flag game.
        /// </summary>
        public FlagSystem FlagSystem { get; private set; }

        /// <summary>
        /// Contains information about the Everybody Edits world that the bot joined.
        /// </summary>
        public WorldInformation JoinedWorld { get; private set; }

        /// <summary>
        /// Event handler for when a player enters or leaves God mode in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that entered or left God mode.</param>
        public delegate void GodModeToggledHandler(CaptureTheFlagBot ctfBot, Player player);
        public event GodModeToggledHandler OnGodModeToggled;

        /// <summary>
        /// Event handler for when a player receives or loses an effect in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="eventArgs">The arguments for when the player received/lost an effect.</param>
        public delegate void OnEffectToggledHandler(CaptureTheFlagBot ctfBot, EffectToggledEventArgs eventArgs);
        public event OnEffectToggledHandler OnEffectToggled;

        /// <summary>
        /// Event handler for when a player joins the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that joined the world.</param>
        public delegate void PlayerJoinedHandler(CaptureTheFlagBot ctfBot, Player player);
        public event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Event handler for when a player leaves the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that left the world.</param>
        public delegate void PlayerLeftHandler(CaptureTheFlagBot ctfBot, Player player);
        public event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Event handler for when a player moves in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that moved.</param>
        public delegate void PlayerMovedHandler(CaptureTheFlagBot ctfBot, Player player);
        public event PlayerMovedHandler OnPlayerMoved;

        /// <summary>
        /// Event handler for when a player is reset in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="eventArgs">The arguments for when the player (or players) was/were reset.</param>
        public delegate void PlayerResetHandler(CaptureTheFlagBot ctfBot, PlayerResetEventArgs eventArgs);
        public event PlayerResetHandler OnPlayerReset;

        /// <summary>
        /// Event handler for when a player changes their smiley in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that changed their smiley.</param>
        public delegate void SmileyChangedHandler(CaptureTheFlagBot ctfBot, Player player);
        public event SmileyChangedHandler OnSmileyChanged;

        /// <summary>
        /// Event handler for when a player joins or leaves a team in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that joined or left a team.</param>
        public delegate void TeamChangedHandler(CaptureTheFlagBot ctfBot, Player player);
        public event TeamChangedHandler OnTeamChanged;

        /// <summary>
        /// Event handler for when the bot is disconnected from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="message">The reason why the bot was disconnected.</param>
        private void OnDisconnect(object sender, string message)
        {
            // Unsubscribe from events
            connection.OnDisconnect -= OnDisconnect;
            connection.OnMessage -= OnMessage;

            FinishedInit = false;
            JoinedWorld.Players.Clear();

            MySqlDatabase.Save();
            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Disconnected from the Everybody Edits world (Reason: {message}).");

            // Only reconnect if the bot was not disconnected on purpose
            if (AutoReconnectOnDisconnect && message != "Disconnect")
            {
                Logger.WriteLog(LogType.EverybodyEditsMessage, "Auto reconnecting...");

                Connect(connectionInformation);
            }
        }

        /// <summary>
        /// Event handler for every time the bot receives a message from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="message">The message object that contains data about the message received.</param>
        private void OnMessage(object sender, Message message)
        {
            switch (message.Type)
            {
                case EverybodyEditsMessage.ClearWorld:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int blockId;

                            for (int layer = 0; layer < JoinedWorld.Blocks.GetLength(0); layer++)
                            {
                                for (int x = 0; x < JoinedWorld.Blocks.GetLength(1); x++)
                                {
                                    for (int y = 0; y < JoinedWorld.Blocks.GetLength(2); y++)
                                    {
                                        if (x == 0 || y == 0 || x == JoinedWorld.Width - 1 || y == JoinedWorld.Height - 1) // Border block
                                        {
                                            blockId = message.GetInt(2);
                                        }
                                        else // Fill block
                                        {
                                            blockId = message.GetInt(3);
                                        }

                                        JoinedWorld.Blocks[layer, x, y] = new Block(blockId);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.ChatMessage:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            string chatMessage = message.GetString(1);
                            ParsedCommand parsedCommand = Command.GetParsedCommand(chatMessage);

                            if (parsedCommand != null)
                            {
                                foreach (Command command in BotCommands)
                                {
                                    if (command.Handle(this, JoinedWorld.Players[playerId], parsedCommand)) // Break out of foreach loop if the command was successfully handled
                                    {
                                        break;
                                    }
                                }
                            }

                            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Player {JoinedWorld.Players[playerId].Username} said: {chatMessage}");
                        }
                    }
                    break;
                case EverybodyEditsMessage.EditRightsChanged:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            bool canEdit = message.GetBoolean(1);

                            JoinedWorld.Players[playerId].CanToggleGodMode = canEdit; // A player that can edit will ALWAYS have access to God mode
                        }
                    }
                    break;
                case EverybodyEditsMessage.Effect:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            int effectId = message.GetInt(1);
                            bool isEnabled = message.GetBoolean(2);

                            if (effectId == (int)Effect.Curse && !isEnabled)
                            {
                                JoinedWorld.LastCurseRemoveTickMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            }
                            else if (effectId == (int)Effect.Zombie)
                            {
                                JoinedWorld.Players[playerId].IsZombie = isEnabled;
                            }

                            OnEffectToggled?.Invoke(this, new EffectToggledEventArgs(JoinedWorld.Players[playerId], (Effect)effectId, isEnabled));
                        }
                    }
                    break;
                case EverybodyEditsMessage.GodModeToggled:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            bool isGodModeEnabled = message.GetBoolean(1);
                            JoinedWorld.Players[playerId].IsInGodMode = isGodModeEnabled;

                            OnGodModeToggled?.Invoke(this, JoinedWorld.Players[playerId]);
                        }
                    }
                    break;
                case EverybodyEditsMessage.InitBegin:
                    {
                        JoinedWorld.Width = message.GetInt(18);
                        JoinedWorld.Height = message.GetInt(19);
                        JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, JoinedWorld.Width, JoinedWorld.Height);

                        Send(EverybodyEditsMessage.InitEnd);
                    }
                    break;
                case EverybodyEditsMessage.InitEnd:
                    {
                        SetWorldTitle($"{WorldTitle} [ON]");
                        SetGodMode(true);
                        Move(JoinLocation);
                        SayChatMessage("Connected!");

                        FinishedInit = true;

                        Logger.WriteLog(LogType.EverybodyEditsMessage, "Connected to Everybody Edits successfully!");
                    }
                    break;
                case EverybodyEditsMessage.PlayerJoinedWorld:
                    {
                        int playerId = message.GetInt(0);

                        if (!JoinedWorld.Players.ContainsKey(playerId))
                        {
                            string username = message.GetString(1);
                            int smileyId = message.GetInt(3);
                            double xLoc = Math.Round(message.GetDouble(4) / 16.0);
                            double yLoc = Math.Round(message.GetDouble(5) / 16.0);
                            bool isInGodMode = message.GetBoolean(6);
                            int teamId = message.GetInt(15);
                            bool canToggleGodMode = message.GetBoolean(23);

                            JoinedWorld.Players.Add(playerId, new Player(username,
                                smileyId,
                                new Point((int)xLoc, (int)yLoc),
                                isInGodMode,
                                (Team)teamId,
                                canToggleGodMode));

                            // Add new players to the database
                            if (MySqlDatabase.Loaded)
                            {
                                if (MySqlDatabase.PlayerExists(username))
                                {
                                    PlayerData playerData = MySqlDatabase.GetRow(username);

                                    // Kick the player if they are banned
                                    if (playerData != null)
                                    {
                                        if (playerData.IsBanned)
                                        {
                                            KickPlayer(username, "You are banned from this world");
                                        }
                                    }
                                }
                                else
                                {
                                    if (!JoinedWorld.Players[playerId].IsGuest)
                                    {
                                        MySqlDatabase.AddNewPlayer(username, false);

                                        SendPrivateMessage(JoinedWorld.Players[playerId], "Welcome newcomer! Type !help to learn how to play in this world.");
                                    }
                                }
                            }

                            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Player {JoinedWorld.Players[playerId].Username} (id: {playerId}) has joined the world.");

                            OnPlayerJoined?.Invoke(this, JoinedWorld.Players[playerId]);
                        }
                    }
                    break;
                case EverybodyEditsMessage.MorphableBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            int morphId = message.GetInt(3);
                            int layer = message.GetInt(4);

                            JoinedWorld.Blocks[layer, xLoc, yLoc] = new MorphableBlock(blockId, morphId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.MusicBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            int soundId = message.GetInt(3);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new MusicBlock(blockId, soundId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.NonPlayableCharacterBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            string name = message.GetString(3);
                            string[] messages = new string[] { message.GetString(4), message.GetString(5), message.GetString(6) };

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new NonPlayableCharacterBlock(blockId, name, messages);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlaceBlock:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int layerId = message.GetInt(0);
                            int xLoc = message.GetInt(1);
                            int yLoc = message.GetInt(2);
                            int blockId = message.GetInt(3);

                            JoinedWorld.Blocks[layerId, xLoc, yLoc] = new Block(blockId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerLeftWorld:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            Player player = JoinedWorld.Players[playerId];

                            if (player.HasEnemyFlag(this))
                            {
                                FlagSystem.Flags[player.Team.GetOppositeTeam()].Return(this, null, false);
                            }

                            JoinedWorld.Players.Remove(playerId);
                            OnPlayerLeft?.Invoke(this, player);

                            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Player {player.Username} (id: {playerId}) has left the world.");
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerMoved:
                case EverybodyEditsMessage.PlayerTeleported:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            JoinedWorld.Players[playerId].UpdateMovementInformation(message);

                            OnPlayerMoved?.Invoke(this, JoinedWorld.Players[playerId]);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerReset:
                    {
                        if (message.Count >= 6)
                        {
                            List<Player> resetPlayersList = new List<Player>();
                            bool propertiesReset = message.GetBoolean(0);

                            for (uint i = 2; i <= message.Count - 4; i += 4)
                            {
                                int playerId = message.GetInt(i);

                                if (JoinedWorld.Players.ContainsKey(playerId))
                                {
                                    double xLoc = Math.Round(message.GetDouble(i + 1) / 16.0);
                                    double yLoc = Math.Round(message.GetDouble(i + 2) / 16.0);

                                    JoinedWorld.Players[playerId].UpdateLocation((int)xLoc, (int)yLoc);

                                    if (propertiesReset)
                                    {
                                        JoinedWorld.Players[playerId].IsZombie = false;
                                        JoinedWorld.Players[playerId].Team = Team.None;
                                    }

                                    resetPlayersList.Add(JoinedWorld.Players[playerId]);
                                }
                            }

                            OnPlayerReset?.Invoke(this, new PlayerResetEventArgs(propertiesReset, resetPlayersList));
                        }
                    }
                    break;
                case EverybodyEditsMessage.ReloadWorld:
                    {
                        JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, JoinedWorld.Width, JoinedWorld.Height);
                    }
                    break;
                case EverybodyEditsMessage.SmileyChanged:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            int smileyId = message.GetInt(1);

                            if (smileyId != JoinedWorld.Players[playerId].SmileyId) // This is to prevent a player spamming
                            {
                                JoinedWorld.Players[playerId].SmileyId = smileyId;
                                OnSmileyChanged?.Invoke(this, JoinedWorld.Players[playerId]);
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.SystemMessage:
                    {
                        string[] messageWords = message.GetString(1).Split();

                        if (messageWords[1] == "kicked")
                        {
                            string kickedUsername = messageWords[2];
                            int playerId = JoinedWorld.GetPlayerId(kickedUsername);

                            if (playerId != -1)
                            {
                                JoinedWorld.Players.Remove(playerId);
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.SignBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            string text = message.GetString(3);
                            int signColour = message.GetInt(4);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new SignBlock(blockId, text, signColour);
                        }
                    }
                    break;
                case EverybodyEditsMessage.TeamChanged:
                    {
                        int playerId = message.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            int teamId = message.GetInt(1);
                            JoinedWorld.Players[playerId].Team = (Team)teamId;

                            OnTeamChanged?.Invoke(this, JoinedWorld.Players[playerId]);
                        }
                    }
                    break;
                case EverybodyEditsMessage.WorldPortalBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            string targetWorldId = message.GetString(3);
                            int targetSpawnId = message.GetInt(4);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new WorldPortalBlock(blockId, targetWorldId, targetSpawnId);
                        }
                    }
                    break;
            }
        }
    }
}