// File Name:     BotEventHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Data;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.GameMechanics;
using Everybody_Edits_CTF.Enums;
using Everybody_Edits_CTF.Helpers;
using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Everybody_Edits_CTF.Core.Bot
{
    public class BotEventHandler
    {
        /// <summary>
        /// Contains all the players currently in the Everybody Edits world. The integer is the Player id while the Player is the object that contains data about the
        /// player.
        /// </summary>
        public Dictionary<int, Player> PlayersInWorld { get; private set; } = new Dictionary<int, Player>();

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

            PlayersDatabaseTable.Save();
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
                        CaptureTheFlagBot.SendChatMessage("Connected!");
                        CaptureTheFlagBot.SetWorldTitle(true);
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
                                PlayersDatabaseTable.AddNewUser(username);

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

                            if (PlayersInWorld[playerId].IsPlayingGame && smileyId == (int)Smilies.Nurse)
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

                            Traps.Handle(PlayersInWorld[playerId]);
                            RoomEntrances.Handle(PlayersInWorld[playerId]);

                            CaptureTheFlag.CaptureFlag(PlayersInWorld[playerId]);
                            CaptureTheFlag.TakeFlag(PlayersInWorld[playerId]);

                            // Attack/heal system
                            foreach (Player otherPlayer in PlayersInWorld.Values)
                            {
                                if (otherPlayer == PlayersInWorld[playerId])
                                {
                                    continue;
                                }

                                AttackSystem.Handle(PlayersInWorld[playerId], otherPlayer);

                                // Heal teammate
                                if (!TeamHelper.IsEnemyPlayer(PlayersInWorld[playerId].Team, otherPlayer.Team)
                                    && PlayersInWorld[playerId].IsNearPlayer(otherPlayer)
                                    && otherPlayer.Health < 100
                                    && PlayersInWorld[playerId].SmileyId == (int)Smilies.Nurse)
                                {
                                    if (otherPlayer.Heal()) // Health restored fully
                                    {
                                        CaptureTheFlagBot.SendPrivateMessage(otherPlayer, $"You were healed player {PlayersInWorld[playerId].Username}");
                                        CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"You fully healed player {otherPlayer.Username}");
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.ChatMessage:
                    {
                        int playerId = m.GetInt(0);
                        string msg = m.GetString(1);
                        string[] cmdTokens = msg.Split(' ');

                        if (PlayersInWorld.ContainsKey(playerId))
                        {
                            if (CommandHelper.IsBotCommand(cmdTokens[0]))
                            {
                                string cmd = msg.Substring(1, cmdTokens[0].Length - 1).ToLower();

                                switch (cmd)
                                {
                                    case "amiadmin":
                                        {
                                            string result = PlayersInWorld[playerId].IsAdmin ? "You are an administrator." : "You are not an administrator.";

                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], result);
                                        }
                                        break;
                                    case "blueflag":
                                    case "redflag":
                                        {
                                            Team targetTeam = cmd == "blueflag" ? Team.Blue : Team.Red;
                                            string flagHolder = "";

                                            foreach (Player enemyPlayer in PlayersInWorld.Values)
                                            {
                                                if (enemyPlayer.Team == targetTeam && enemyPlayer.HasEnemyFlag)
                                                {
                                                    flagHolder = enemyPlayer.Username;
                                                    break;
                                                }
                                            }

                                            string msgToSend = flagHolder != "" ? $"Player {flagHolder} has the blue flag." : $"No one has blue flag.";
                                            CaptureTheFlagBot.SendChatMessage(msgToSend);
                                        }
                                        break;
                                    case "coins":
                                        {
                                            if (PlayersDatabaseTable.Loaded)
                                            {
                                                PlayerDatabaseRow row = PlayersDatabaseTable.GetPlayerDatabaseRow(PlayersInWorld[playerId].Username);

                                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"You currently have {row.Coins} coin{(row.Coins == 1 ? "" : "s")}.");
                                            }
                                        }
                                        break;
                                    case "disconnect":
                                        {
                                            if (PlayersInWorld[playerId].IsAdmin)
                                            {
                                                CaptureTheFlagBot.Disconnect();
                                            }
                                            else
                                            {
                                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"You don't have permission to execute this command.");
                                            }
                                        }
                                        break;
                                    case "gamefund":
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"The game fund is currently: {CaptureTheFlag.GameFund} coins.");
                                        }
                                        break;
                                    case "heal":
                                        {
                                            string resultMsg = "You cannot heal yourself because you are not inside your base!"; // Default message if the player is not inside their base

                                            if ((PlayersInWorld[playerId].Team == Team.Blue && PlayersInWorld[playerId].IsInBlueBase)
                                                || (PlayersInWorld[playerId].Team == Team.Red && PlayersInWorld[playerId].IsInRedBase))
                                            {
                                                PlayersInWorld[playerId].RestoreHealth();

                                                resultMsg = "Success! Your health was restored fully.";
                                            }

                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], resultMsg);
                                        }
                                        break;
                                    case "health":
                                    case "hp":
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Your current health is: {PlayersInWorld[playerId].Health} HP.");
                                        }
                                        break;
                                    case "help":
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Command prefixes: . > ! #");
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Commands: amiadmin, disconnect, heal, health, help, lobby, scores, suicide, blueflag, redflag");
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Use nurse smiley to heal your teammates!");
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Bot is still work in progress so some things might be glitchy/not work.");
                                        }
                                        break;
                                    case "kick":
                                        {
                                            if (PlayersInWorld[playerId].IsAdmin)
                                            {
                                                if (cmdTokens.Length >= 2)
                                                {
                                                    string playerToKick = cmdTokens[1];
                                                    string reason = "";

                                                    if (cmdTokens.Length >= 3)
                                                    {
                                                        for (int i = 2; i < cmdTokens.Length; i++)
                                                        {
                                                            reason += cmdTokens[i] + " ";
                                                        }
                                                    }

                                                    CaptureTheFlagBot.KickPlayer(playerToKick, reason);
                                                }
                                                else
                                                {
                                                    CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Insufficient amount of parameters for command.");
                                                }
                                            }
                                            else
                                            {
                                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"You don't have permission to execute this command.");
                                            }
                                        }
                                        break;
                                    case "lobby":
                                    case "quit":
                                        {
                                            CaptureTheFlagBot.TeleportPlayer(PlayersInWorld[playerId], 199, 1);
                                        }
                                        break;
                                    case "maxflags":
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"The maximum number of flags to win is {GameSettings.MaxScoreToWin} flag{(GameSettings.MaxScoreToWin == 1 ? "" : "s")}.");
                                        }
                                        break;
                                    case "scores":
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"Blue: {CaptureTheFlag.BlueTeamScore} | Red: {CaptureTheFlag.RedTeamScore}");
                                        }
                                        break;
                                    case "suicide":
                                        {
                                            CaptureTheFlagBot.KillPlayer(PlayersInWorld[playerId], null, DeathReason.Suicide);
                                        }
                                        break;
                                    case "totalwins":
                                        {
                                            if (PlayersDatabaseTable.Loaded)
                                            {
                                                PlayerDatabaseRow row = PlayersDatabaseTable.GetPlayerDatabaseRow(PlayersInWorld[playerId].Username);

                                                CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"You have won {row.TotalWins} time{(row.TotalWins == 1 ? "" : "s")}.");
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(PlayersInWorld[playerId], $"The command \"{cmd}\" is invalid!");
                                        }
                                        break;
                                }
                            }
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
                            bool playingGame = joinedTeam != Team.None;

                            if (playingGame)
                            {
                                string resultMsg = $"You joined the {TeamHelper.EnumToString(joinedTeam)} team!";

                                if (GameSettings.AutoBalanceTeams && TeamHelper.TotalPlayers(PlayersInWorld, joinedTeam) > TeamHelper.TotalPlayers(PlayersInWorld, TeamHelper.GetOppositeTeam(joinedTeam)))
                                {
                                    resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                                    int teleX = joinedTeam == Team.Blue ? 398 : 1;
                                    int teleY = 1;

                                    CaptureTheFlagBot.TeleportPlayer(PlayersInWorld[playerId], teleX, teleY);
                                }
                                else
                                {
                                    PlayersInWorld[playerId].Team = joinedTeam;
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
                                PlayersInWorld[playerId].RestoreHealth();

                                CaptureTheFlag.ReturnFlag(PlayersInWorld[playerId], true);
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerTeleported:
                    {
                        int playerId = m.GetInt(0);
                        int xLoc = m.GetInt(1);
                        int yLoc = m.GetInt(2);

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