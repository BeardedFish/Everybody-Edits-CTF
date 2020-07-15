// File Name:     BotEventHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.GameMechanics;
using Everybody_Edits_CTF.Core.GameMechanics.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using Everybody_Edits_CTF.Logging;
using Everybody_Edits_CTF.Logging.Enums;
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot
{
    public class BotEventHandler
    {
        /// <summary>
        /// Contains all the players currently in the Everybody Edits world. The integer is the Player id while the Player is the object that contains data about the
        /// player.
        /// </summary>
        public Dictionary<int, Player> PlayersInWorld { get; private set; } = new Dictionary<int, Player>();

        /// <summary>
        /// Handles all messages that a Capture the Flag bot receives.
        /// </summary>
        /// <param name="connection">The connection of the bot to Everybody Edits.</param>
        public BotEventHandler(Connection connection)
        {
            connection.OnDisconnect += OnDisconnect;
            connection.OnMessage += OnMessage;
        }

        /// <summary>
        /// Event handler for when the bot is disconnected from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="message">The reason why the bot was disconnected.</param>
        private void OnDisconnect(object sender, string message)
        {
            PlayersInWorld.Clear();

            CaptureTheFlag.ResetGameStatistics();
            PlayersDatabaseTable.Save();
            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Disconnected from the Everybody Edits world (Reason: {message}).");

            if (BotSettings.AutoReconnectOnDisconnect && message != string.Empty)
            {
                Logger.WriteLog(LogType.EverybodyEditsMessage, $"Auto reconnecting...");

                CaptureTheFlagBot.Connect();
            }
        }

        /// <summary>
        /// Event handler for every time the bot receives a message from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="m">The message object that contains data about the message received.</param>
        private void OnMessage(object sender, Message m)
        {
            switch (m.Type)
            {
                case EverybodyEditsMessage.InitBegin:
                    {
                        CaptureTheFlagBot.Send(EverybodyEditsMessage.InitEnd);
                    }
                    break;
                case EverybodyEditsMessage.InitEnd:
                    {
                        CaptureTheFlagBot.SetWorldTitle($"{BotSettings.WorldTitle} [ON]");
                        CaptureTheFlagBot.SetGodMode(true);
                        CaptureTheFlagBot.Move(new Point(0, 0));
                        CaptureTheFlagBot.SendChatMessage("Connected!");

                        Logger.WriteLog(LogType.EverybodyEditsMessage, "Connected to Everybody Edits succesfully!");
                    }
                    break;
                case EverybodyEditsMessage.PlayerJoinedWorld:
                    {
                        int playerId = m.GetInt(0);
                        string username = m.GetString(1);
                        int smileyId = m.GetInt(3);
                        double xLoc = Math.Round(m.GetDouble(4) / 16.0);
                        double yLoc = Math.Round(m.GetDouble(5) / 16.0);
                        int teamId = m.GetInt(15);

                        PlayersInWorld.Add(playerId, new Player(username, smileyId, new Point((int)xLoc, (int)yLoc), TeamHelper.IdToEnum(teamId)));

                        if (PlayersDatabaseTable.Loaded)
                        {
                            if (!PlayersInWorld[playerId].IsGuest && !PlayersDatabaseTable.PlayerExists(username))
                            {
                                PlayersDatabaseTable.AddNewPlayer(username);

                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], "Welcome newcomer! Type !help to learn how to play in this world.");
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerLeftWorld:
                    {
                        int playerId = m.GetInt(0);

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            CaptureTheFlag.ReturnFlag(PlayersInWorld[playerId], false);

                            PlayersInWorld.Remove(playerId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.SmileyChanged:
                    {
                        int playerId = m.GetInt(0);
                        int smileyId = m.GetInt(1);

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            PlayersInWorld[playerId].SmileyId = smileyId;

                            if (PlayersInWorld[playerId].IsPlayingGame && smileyId == (int)Smiley.Nurse)
                            {
                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], "You are now a healer for your team!");
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.GodModeToggled:
                {
                        int playerId = m.GetInt(0);
                        bool isInGodMode = m.GetBoolean(1);

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            PlayersInWorld[playerId].IsInGodMode = isInGodMode;

                            AntiCheat.Handle(PlayersInWorld[playerId]);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerMoved:
                    {
                        int playerId = m.GetInt(0);
                        double xLoc = Math.Round(m.GetDouble(1) / 16.0);
                        double yLoc = Math.Round(m.GetDouble(2) / 16.0);
                        int horizontalDir = m.GetInt(7);
                        int verticalDir = m.GetInt(8);

                        if (PlayersInWorld.ContainsKey(playerId) && PlayersInWorld[playerId].IsPlayingGame)
                        {
                            PlayersInWorld[playerId].UpdateLocation((int)xLoc, (int)yLoc);
                            
                            PlayersInWorld[playerId].HorizontalDirection = (HorizontalDirection)horizontalDir;
                            PlayersInWorld[playerId].VerticalDirection = (VerticalDirection)verticalDir;

                            if (!PlayersInWorld[playerId].IsRespawning)
                            {
                                Traps.Handle(PlayersInWorld[playerId]);
                                RoomEntrances.Handle(PlayersInWorld[playerId]);
                                Shop.Handle(PlayersInWorld[playerId]);

                                CaptureTheFlag.CaptureFlag(PlayersInWorld[playerId]);
                                CaptureTheFlag.TakeFlag(PlayersInWorld[playerId]);

                                foreach (Player otherPlayer in PlayersInWorld.Values)
                                {
                                    if (otherPlayer == PlayersInWorld[playerId])
                                    {
                                        continue;
                                    }

                                    AttackSystem.Handle(PlayersInWorld[playerId], otherPlayer);
                                    HealingSystem.Handle(PlayersInWorld[playerId], otherPlayer);
                                }
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.ChatMessage:
                    {
                        int playerId = m.GetInt(0);
                        string msg = m.GetString(1);

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            ChatMessageCommands.Handle(PlayersInWorld[playerId], msg);
                        }
                    }
                    break;
                case EverybodyEditsMessage.TeamChanged:
                    {
                        int playerId = m.GetInt(0);
                        int teamId = m.GetInt(1);

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            Team joinedTeam = (Team)teamId;

                            PlayersInWorld[playerId].Team = joinedTeam;

                            if (PlayersInWorld[playerId].IsPlayingGame)
                            {
                                string resultMsg = $"You joined the {TeamHelper.EnumToString(joinedTeam)} team!";
                                int joinedTeamTotalPlayers = TeamHelper.TotalPlayers(PlayersInWorld, joinedTeam) - 1;
                                int oppositeTeamTotalPlayers = TeamHelper.TotalPlayers(PlayersInWorld, TeamHelper.GetOppositeTeam(joinedTeam));

                                if (GameSettings.AutoBalanceTeams)
                                {
                                    if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
                                    {
                                        resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                                        int teleX = joinedTeam == Team.Blue ? 398 : 1;
                                        int teleY = 1;

                                        CaptureTheFlagBot.TeleportPlayer(PlayersInWorld[playerId], teleX, teleY);
                                    }
                                }

                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], resultMsg);
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerRespawn:
                    {
                        if (m.Count == 6) // A count of six means that a played died
                        {
                            int playerId = m.GetInt(2);
                            double xLoc = Math.Round(m.GetDouble(3) / 16.0);
                            double yLoc = Math.Round(m.GetDouble(4) / 16.0);

                            if (PlayersInWorld.ContainsKey(playerId))
                            {
                                PlayersInWorld[playerId].UpdateLocation((int)xLoc, (int)yLoc);
                                PlayersInWorld[playerId].Respawn();
                                PlayersInWorld[playerId].RestoreHealth();

                                if (PlayersInWorld[playerId].LastAttacker != null)
                                {
                                    CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"You were killed by player {PlayersInWorld[playerId].LastAttacker.Username}!");
                                    CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId].LastAttacker, $"You killed player {PlayersInWorld[playerId].Username}!");

                                    if (PlayersDatabaseTable.Loaded)
                                    {
                                        PlayerDatabaseRow playerData = PlayersDatabaseTable.GetPlayerDatabaseRow(PlayersInWorld[playerId].LastAttacker.Username);

                                        if (playerData != null)
                                        {
                                            playerData.TotalKills++;
                                        }
                                    }

                                    PlayersInWorld[playerId].LastAttacker = null;

                                    CaptureTheFlag.IncreaseGameFund(GameFundIncreaseReason.PlayerKilledEnemy);
                                }

                                CaptureTheFlag.ReturnFlag(PlayersInWorld[playerId], true);
                                CaptureTheFlagBot.RemoveEffects(PlayersInWorld[playerId]);
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerTeleported:
                    {
                        int playerId = m.GetInt(0);
                        int xLoc = (int)Math.Round(m.GetInt(1) / 16.0);
                        int yLoc = (int)Math.Round(m.GetInt(2) / 16.0);

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            PlayersInWorld[playerId].UpdateLocation(xLoc, yLoc);
                        }
                    }
                    break;
            }
        }
    }
}