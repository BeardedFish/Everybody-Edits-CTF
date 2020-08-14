﻿// File Name:     GameCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Commands;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class GameCommands : Command
    {
        public GameCommands() : base(new string[] { "blueflag", "redflag", "dropflag", "gamefund", "heal", "health", "hp", "lobby", "quit", "maxflags", "scores", "suicide" })
        {

        }

        public override bool Handle(Player player, ParsedCommand parsedCommand)
        {
            if (parsedCommand != null && ValidCommands.Contains(parsedCommand.Command))
            {
                if (player.IsPlayingGame)
                {
                    switch (parsedCommand.Command)
                    {
                        case "blueflag":
                        case "redflag":
                            {
                                Team targetTeam = parsedCommand.Command == "blueflag" ? Team.Blue : Team.Red;
                                string flagHolderUsername = CtfGameRound.FlagSystem.Flags[targetTeam].Holder == null ? null : CtfGameRound.FlagSystem.Flags[targetTeam].Holder.Username;
                                string teamName = TeamHelper.EnumToString(targetTeam);
                                string msgToSend = flagHolderUsername != null ? $"Player {flagHolderUsername} has the {teamName} flag." : $"No one has {teamName} flag.";

                                CtfBot.SendChatMessage(msgToSend);
                            }
                            break;
                        case "dropflag":
                            {
                                Team enemyTeam = TeamHelper.GetOppositeTeam(player.Team);
                                Flag enemyTeamFlag = CtfGameRound.FlagSystem.Flags[enemyTeam];

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
                                            CtfBot.SendPrivateMessage(player, "You can't drop the flag here!");
                                        }
                                    }
                                }
                                else
                                {
                                    CtfBot.SendPrivateMessage(player, "You are not holding the enemy flag.");
                                }
                            }
                            break;
                        case "gamefund":
                            {
                                CtfBot.SendPrivateMessage(player, $"The game fund is currently: {CtfBot.CurrentGameRound.GameFund} coins.");
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

                                CtfBot.SendPrivateMessage(player, resultMsg);
                            }
                            break;
                        case "health":
                        case "hp":
                            {
                                CtfBot.SendPrivateMessage(player, $"Your current health is: {player.Health} HP.");
                            }
                            break;
                        case "lobby":
                        case "quit":
                            {
                                player.GoToLobby();
                            }
                            break;
                        case "maxflags":
                            {
                                CtfBot.SendPrivateMessage(player, $"The maximum number of flags to win is {GameSettings.MaxScoreToWin} flag{(GameSettings.MaxScoreToWin == 1 ? "" : "s")}.");
                            }
                            break;
                        case "scores":
                            {
                                CtfBot.SendPrivateMessage(player, CtfBot.CurrentGameRound.GetScoresString());
                            }
                            break;
                        case "suicide":
                            {
                                if (!player.IsRespawning)
                                {
                                    player.Die();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    CtfBot.SendPrivateMessage(player, "You must be on either team blue or team red to use this command.");
                }

                return true;
            }

            return false;
        }
    }
}