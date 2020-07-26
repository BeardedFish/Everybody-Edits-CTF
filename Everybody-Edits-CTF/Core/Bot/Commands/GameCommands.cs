// File Name:     GameCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, July 5, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.GameMechanics;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.Bot.Commands
{
    public static class GameCommands
    {
        /// <summary>
        /// An array of type <see cref="string"/> which contains the valid game commands.
        /// </summary>
        public static readonly string[] ValidCommands =
        {
            "blueflag",
            "redflag",
            "dropflag",
            "gamefund",
            "heal",
            "health",
            "hp",
            "lobby",
            "quit",
            "maxflags",
            "scores",
            "suicide"
        };

        /// <summary>
        /// Handles a game command that an Everybody Edits player wants to execute.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        /// <param name="cmd">The command that the player wants to execute.</param>
        /// <param name="cmdTokens">The command tokens (strings seperated by a space character).</param>
        public static void Handle(Player player, string cmd, string[] cmdTokens)
        {
            if (player.IsPlayingGame)
            {
                if (player.IsRespawning)
                {
                    // TODO: Probably should send the player a private message...

                    return;
                }

                switch (cmd)
                {
                    case "blueflag":
                    case "redflag":
                        {
                            Team targetTeam = cmd == "blueflag" ? Team.Blue : Team.Red;
                            string flagHolderUsername = CaptureTheFlag.Flags[targetTeam].Holder == null ? null : CaptureTheFlag.Flags[targetTeam].Holder.Username;
                            string teamName = TeamHelper.EnumToString(targetTeam);
                            string msgToSend = flagHolderUsername != null ? $"Player {flagHolderUsername} has the {teamName} flag." : $"No one has {teamName} flag.";

                            CaptureTheFlagBot.SendChatMessage(msgToSend);
                        }
                        break;
                    case "dropflag":
                        {
                            Team enemyTeam = TeamHelper.GetOppositeTeam(player.Team);
                            Flag enemyTeamFlag = CaptureTheFlag.Flags[enemyTeam];

                            if (enemyTeamFlag.Holder == player)
                            {
                                if (JoinedWorld.Blocks != null)
                                {
                                    if (JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y].Id == 0)
                                    {
                                        enemyTeamFlag.Drop();
                                    }
                                    else
                                    {
                                        CaptureTheFlagBot.SendPrivateMessage(player, "You can't drop the flag here!");
                                    }
                                }
                            }
                            else
                            {
                                CaptureTheFlagBot.SendPrivateMessage(player, "You are not holding the enemy flag.");
                            }
                        }
                        break;
                    case "gamefund":
                        {
                            CaptureTheFlagBot.SendPrivateMessage(player, $"The game fund is currently: {GameFund.CoinsRaised} coins.");
                        }
                        break;
                    case "heal":
                        {
                            string resultMsg = "You cannot heal yourself because you are not inside your base!"; // Default message if the player is not inside their base

                            if ((player.Team == Team.Blue && player.IsInBlueBase)
                                || (player.Team == Team.Red && player.IsInRedBase))
                            {
                                player.RestoreHealth();

                                resultMsg = "Success! Your health was restored fully.";
                            }

                            CaptureTheFlagBot.SendPrivateMessage(player, resultMsg);
                        }
                        break;
                    case "health":
                    case "hp":
                        {
                            CaptureTheFlagBot.SendPrivateMessage(player, $"Your current health is: {player.Health} HP.");
                        }
                        break;
                    case "lobby":
                    case "quit":
                        {
                            CaptureTheFlagBot.TeleportPlayer(player, 199, 1);
                        }
                        break;
                    case "maxflags":
                        {
                            CaptureTheFlagBot.SendPrivateMessage(player, $"The maximum number of flags to win is {GameSettings.MaxScoreToWin} flag{(GameSettings.MaxScoreToWin == 1 ? "" : "s")}.");
                        }
                        break;
                    case "scores":
                        {
                            CaptureTheFlagBot.SendPrivateMessage(player, $"Blue: {CaptureTheFlag.Scores[Team.Blue]} | Red: {CaptureTheFlag.Scores[Team.Red]}");
                        }
                        break;
                    case "suicide":
                        {
                            player.Die();
                        }
                        break;
                }
            }
            else
            {
                CaptureTheFlagBot.SendPrivateMessage(player, "You must be on either team blue or team red to use this command.");
            }
        }
    }
}