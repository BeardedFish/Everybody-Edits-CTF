// File Name:     RegularCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Commands;
using Everybody_Edits_CTF.Core.Database;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class RegularCommands : Command
    {
        /// <summary>
        /// Contains regular bot commands implementation. A regular command can be executed by any player.
        /// </summary>
        public RegularCommands() : base(new string[] { "amiadmin", "coins", "donatecoins", "help", "spectate", "totalwins", "totallosses", "losses", "wins", "totalkills" })
        {

        }

        /// <summary>
        /// Handles a player executing a regular command.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        /// <param name="parsedCommand">The command being executed.</param>
        /// <returns>
        /// True if the command was succesfully handled, if not, false. A succesful handle is when the parsed command is not equal to null and also the ValidCommands string
        /// array contains the parsed command.
        /// </returns>
        public override bool Handle(Player player, ParsedCommand parsedCommand)
        {
            if (parsedCommand != null && ValidCommands.Contains(parsedCommand.Command))
            {
                switch (parsedCommand.Command)
                {
                    case "amiadmin":
                        {
                            string result = player.IsAdmin ? "You are an administrator." : "You are not an administrator.";

                            CtfBot.SendPrivateMessage(player, result);
                        }
                        break;
                    case "coins":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                PlayerDatabaseRow row = PlayersDatabaseTable.GetRow(player.Username);

                                CtfBot.SendPrivateMessage(player, $"You currently have {row.Coins} coin{(row.Coins == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                    case "donatecoins":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                if (parsedCommand.Parameters.Length >= 2)
                                {
                                    if (parsedCommand.Parameters[0] != player.Username)
                                    {
                                        PlayerDatabaseRow playerToDonateData = PlayersDatabaseTable.GetRow(parsedCommand.Parameters[0]);

                                        if (playerToDonateData != null)
                                        {
                                            PlayerDatabaseRow donatorData = PlayersDatabaseTable.GetRow(player.Username);

                                            if (int.TryParse(parsedCommand.Parameters[1], out int amount))
                                            {
                                                if (donatorData.Coins >= amount)
                                                {
                                                    playerToDonateData.Coins += amount;
                                                    donatorData.Coins -= amount;

                                                    CtfBot.SendPrivateMessage(player, $"Success! You donated {amount} coin{(amount == 1 ? "" : "s")} to player {parsedCommand.Parameters[0].ToUpper()}.");
                                                }
                                                else
                                                {
                                                    CtfBot.SendPrivateMessage(player, "You don't have enough coins to donate that amount!");
                                                }
                                            }
                                            else
                                            {
                                                CtfBot.SendPrivateMessage(player, "Third parameter of command is invalid! It must be a number.");
                                            }
                                        }
                                        else
                                        {
                                            CtfBot.SendPrivateMessage(player, $"Player {parsedCommand.Parameters[0].ToUpper()} does not exist.");
                                        }
                                    }
                                    else
                                    {
                                        CtfBot.SendPrivateMessage(player, "You can't donate coins to yourself!");
                                    }
                                }
                                else
                                {
                                    CtfBot.SendPrivateMessage(player, $"Insufficient amount of parameters for command.");
                                }
                            }
                        }
                        break;
                    case "help":
                        {
                            CtfBot.SendPrivateMessage(player, "Command prefixes: . > ! #");

                            CtfBot.SendPrivateMessage(player, "Regular Commands:");
                            //CaptureTheFlagBot.SendPrivateMessage(player, StringArrayToString(RegularCommands.ValidCommands));

                            CtfBot.SendPrivateMessage(player, "Game Commands:");
                            //CaptureTheFlagBot.SendPrivateMessage(player, StringArrayToString(GameCommands.));

                            CtfBot.SendPrivateMessage(player, "Administrator Commands:");
                            //CaptureTheFlagBot.SendPrivateMessage(player, StringArrayToString(AdminCommands.ValidCommands));

                            CtfBot.SendPrivateMessage(player, "Tips:");
                            CtfBot.SendPrivateMessage(player, "- Press arrow keys/WASD keys around an enemy player to attack them.");
                            CtfBot.SendPrivateMessage(player, "- Use nurse smiley to heal your teammates!");
                            CtfBot.SendPrivateMessage(player, "- There is an item shop in the clouds.");
                            CtfBot.SendPrivateMessage(player, "- Watch out for traps around the map.");
                        }
                        break;
                    case "spectate":
                        {
                            if (!player.IsPlayingGame && !player.CanToggleGodMode)
                            {
                                CtfBot.SetForceFly(player, !player.IsInGodMode);
                            }
                            else
                            {
                                string privateMessage = player.IsPlayingGame ? "This command is only available to players not playing!" : "You can toggle God mode! Use that instead.";

                                CtfBot.SendPrivateMessage(player, privateMessage);
                            }
                        }
                        break;
                    case "totalwins":
                    case "totallosses":
                    case "losses":
                    case "wins":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                PlayerDatabaseRow row = PlayersDatabaseTable.GetRow(player.Username);
                                int resultCount = parsedCommand.Command == "totalwins" || parsedCommand.Command == "wins" ? row.TotalWins : row.TotalLosses;
                                string type = parsedCommand.Command == "totalwins" || parsedCommand.Command == "wins" ? "won" : "lost";

                                CtfBot.SendPrivateMessage(player, $"You have {type} {resultCount} time{(row.TotalWins == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                    case "totalkills":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                PlayerDatabaseRow playerData = PlayersDatabaseTable.GetRow(player.Username);

                                CtfBot.SendPrivateMessage(player, $"You have killed a total of {playerData.TotalKills} player{(playerData.TotalKills == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                }

                return true;
            }
            else
            {
                CtfBot.SendPrivateMessage(player, $"The command \"{parsedCommand.Command}\" is invalid!");
            }

            return false;
        }

        /// <summary>
        /// Converts a string array to a single string. Each string item in the string array (excluding the last item) is seperated by a comma and a space.
        /// </summary>
        /// <param name="array">The string array to be converted to a single string.</param>
        /// <returns>A <see cref="string"/> which contains the data of the string array seperated by commas.</returns>
        private static string StringArrayToString(string[] array)
        {
            string result = "";

            for (int i = 0; i < array.Length; i++)
            {
                result += $"{array[i]}{(i == array.Length - 1 ? "" : ", ")}";
            }

            return result;
        }
    }
}