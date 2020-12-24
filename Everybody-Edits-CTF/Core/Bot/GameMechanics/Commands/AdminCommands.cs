// File Name:     AdminCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using System;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class AdminCommands : Command
    {
        public AdminCommands() : base(new string[] { "ban", "disconnect", "kick", "retf", "unban" })
        {

        }

        /// <summary>
        /// Handles a player executing an administrator command. Administrators are defined in the MySql database.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player executing the command.</param>
        /// <param name="parsedCommand">The command being executed.</param>
        /// <returns>
        /// True if the command was succesfully handled, if not, false. A succesful handle is when the parsed command is not equal to null and also the ValidCommands string
        /// array contains the parsed command.
        /// </returns>
        public override bool Handle(CaptureTheFlagBot ctfBot, Player player, ParsedCommand parsedCommand)
        {
            bool canHandle = base.Handle(ctfBot, player, parsedCommand);

            if (canHandle)
            {
                if (MySqlDatabase.Loaded)
                {
                    if (MySqlDatabase.GetRow(player.Username).IsAdministrator)
                    {
                        switch (parsedCommand.Command)
                        {
                            case "ban":
                            case "unban":
                                {
                                    if (parsedCommand.Parameters.Length >= 1)
                                    {
                                        string username = parsedCommand.Parameters[0];
                                        PlayerData playerData = MySqlDatabase.GetRow(username);

                                        if (parsedCommand.Command.Equals("ban", System.StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (playerData != null) // Player exists in the database
                                            {
                                                if (!playerData.IsBanned)
                                                {
                                                    playerData.IsBanned = true;
                                                    ctfBot?.KickPlayer(username, "You've been banned from this world.");
                                                }
                                                else
                                                {
                                                    ctfBot?.SendPrivateMessage(player, $"Player {username.ToUpper()} is already banned!");
                                                }
                                            }
                                            else // Player does not exist in the database
                                            {
                                                MySqlDatabase.AddNewPlayer(username, true);
                                            }

                                            ctfBot?.SendPrivateMessage(player, $"Player {username.ToUpper()} has been banned from the world.");
                                        }
                                        else // The command is "unban"
                                        {
                                            if (playerData != null)
                                            {
                                                if (playerData.IsBanned)
                                                {
                                                    playerData.IsBanned = false;

                                                    ctfBot?.ForgivePlayer(username);
                                                    ctfBot?.SendPrivateMessage(player, $"Player {username.ToUpper()} has been unbanned.");
                                                }
                                                else
                                                {
                                                    ctfBot?.SendPrivateMessage(player, $"Player {username.ToUpper()} is not banned!");
                                                }
                                            }
                                            else
                                            {
                                                ctfBot?.SendPrivateMessage(player, $"Cannot ban player {username.ToUpper()} because they don't exist.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ctfBot?.SendPrivateMessage(player, "Insufficient amount of parameters!");
                                    }
                                }
                                break;
                            case "disconnect":
                                {
                                    ctfBot.Disconnect();
                                }
                                break;
                            case "kick":
                                {
                                    if (parsedCommand.Parameters.Length >= 1)
                                    {
                                        string playerToKick = parsedCommand.Parameters[0];
                                        string reason = "";

                                        if (parsedCommand.Parameters.Length >= 2)
                                        {
                                            for (int i = 2; i < parsedCommand.Parameters.Length; i++)
                                            {
                                                reason += parsedCommand.Parameters[i] + " ";
                                            }
                                        }

                                        ctfBot.KickPlayer(playerToKick, reason);
                                    }
                                    else
                                    {
                                        ctfBot.SendPrivateMessage(player, "Insufficient amount of parameters for command.");
                                    }
                                }
                                break;
                            case "retf": // Return flag
                                {
                                    if (parsedCommand.Parameters.Length >= 1)
                                    {
                                        bool isValidParameter = string.Equals(parsedCommand.Parameters[0], "blue", StringComparison.OrdinalIgnoreCase) || string.Equals(parsedCommand.Parameters[0], "red", StringComparison.OrdinalIgnoreCase);

                                        if (isValidParameter)
                                        {
                                            if (string.Equals(parsedCommand.Parameters[0], "blue", StringComparison.OrdinalIgnoreCase) && ctfBot.FlagSystem.Flags[Team.Blue].IsTaken)
                                            {
                                                ctfBot?.FlagSystem.Flags[Team.Blue].Return(ctfBot, null, false);
                                            }
                                            else if (string.Equals(parsedCommand.Parameters[0], "red", StringComparison.OrdinalIgnoreCase) && ctfBot.FlagSystem.Flags[Team.Red].IsTaken)
                                            {
                                                ctfBot?.FlagSystem.Flags[Team.Red].Return(ctfBot, null, false);
                                            }
                                            else
                                            {
                                                ctfBot?.SendPrivateMessage(player, $"Cannot return the {parsedCommand.Parameters[0].ToLower()} flag as it is already at its base!");
                                            }
                                        }
                                        else // Parameter is not "blue" or "red"
                                        {
                                            ctfBot?.SendPrivateMessage(player, "Unknown flag type.");
                                        }
                                    }
                                    else
                                    {
                                        ctfBot?.SendPrivateMessage(player, "Insufficient amount of parameters for command.");
                                    }
                                }
                                break;
                        }
                    }
                    else // User is not an administrator
                    {
                        ctfBot?.SendPrivateMessage(player, "You don't have permission to execute this command.");
                    }
                }
                else
                {
                    ctfBot?.SendPrivateMessage(player, "Administrator commands are disabled due to the database not being loaded!");
                }
            }

            return canHandle;
        }
    }
}