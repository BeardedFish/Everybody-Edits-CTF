﻿// File Name:     GameCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class GameCommands : Command
    {
        public GameCommands() : base(new string[] { "blueflag", "redflag", "dropflag", "gamefund", "heal", "health", "hp", "lobby", "quit", "maxflags", "scores", "suicide" })
        {

        }

        /// <summary>
        /// Handles a player executing a game command. A game command can only be executed by a player either on the blue team or red team.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player executing the command.</param>
        /// <param name="parsedCommand">The command being executed.</param>
        /// <returns>
        /// True if the command was successfully handled, if not, false. A successful handle is when the parsed command is not equal to null and also the ValidCommands string
        /// array contains the parsed command.
        /// </returns>
        public override bool Handle(CaptureTheFlagBot ctfBot, Player player, ParsedCommand parsedCommand)
        {
            bool canHandle = base.Handle(ctfBot, player, parsedCommand);

            if (canHandle)
            {
                if (player.IsPlayingGame)
                {
                    switch (parsedCommand.Command)
                    {
                        case "blueflag":
                        case "redflag":
                            {
                                Team targetTeam = parsedCommand.Command == "blueflag" ? Team.Blue : Team.Red;
                                string flagHolderUsername = ctfBot.FlagSystem.Flags[targetTeam].Holder?.Username ?? "";
                                string teamName = targetTeam.GetStringName();
                                string msgToSend = !string.IsNullOrEmpty(flagHolderUsername) ? $"Player {flagHolderUsername} has the {teamName} flag." : $"No one has {teamName} flag.";

                                ctfBot.SayChatMessage(msgToSend);
                            }
                            break;
                        case "dropflag":
                            {
                                Team enemyTeam = player.Team.GetOppositeTeam();
                                Flag enemyTeamFlag = ctfBot.FlagSystem.Flags[enemyTeam];

                                if (enemyTeamFlag.Holder == player)
                                {
                                    if (ctfBot.JoinedWorld.Blocks != null)
                                    {
                                        if (ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y].Id == 0)
                                        {
                                            enemyTeamFlag.Drop(ctfBot);
                                        }
                                        else
                                        {
                                            ctfBot.SendPrivateMessage(player, "You can't drop the flag here!");
                                        }
                                    }
                                }
                                else
                                {
                                    ctfBot.SendPrivateMessage(player, "You are not holding the enemy flag.");
                                }
                            }
                            break;
                        case "gamefund":
                            {
                                ctfBot.SendPrivateMessage(player, $"The game fund is currently: {ctfBot.CurrentGameRound.GameFund} coins.");
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

                                ctfBot.SendPrivateMessage(player, resultMsg);
                            }
                            break;
                        case "health":
                        case "hp":
                            {
                                ctfBot.SendPrivateMessage(player, $"Your current health is: {player.Health} HP.");
                            }
                            break;
                        case "lobby":
                        case "quit":
                            {
                                player.GoToLobby(ctfBot);
                            }
                            break;
                        case "maxflags":
                            {
                                ctfBot.SendPrivateMessage(player, $"The maximum number of flags to win is {FlagSystem.MaxScoreToWin} flag{(FlagSystem.MaxScoreToWin == 1 ? "" : "s")}.");
                            }
                            break;
                        case "scores":
                            {
                                ctfBot.SendPrivateMessage(player, ctfBot.CurrentGameRound.GetScoresString());
                            }
                            break;
                        case "suicide":
                            {
                                if (!player.IsRespawning)
                                {
                                    player.Die(ctfBot);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    ctfBot.SendPrivateMessage(player, "You must be on either team blue or team red to use this command.");
                }
            }

            return canHandle;
        }
    }
}