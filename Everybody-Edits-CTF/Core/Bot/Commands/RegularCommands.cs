// File Name:     RegularCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, July 5, 2020

using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.DataStructures;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.Commands
{
    public static class RegularCommands
    {
        public static readonly string[] ValidCommands =
        {
            "amiadmin",
            "coins",
            "donatecoins",
            "help",
            "totalwins",
            "totallosses",
            "losses",
            "wins",
            "totalkills"
        };

        /// <summary>
        /// Handles a regular command that an Everybody Edits player wants to execute.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        /// <param name="cmd">The command that the player wants to execute.</param>
        /// <param name="cmdTokens">The command tokens (strings seperated by a space character).</param>
        public static void Handle(Player player, string cmd, string[] cmdTokens)
        {
            if (ValidCommands.Contains(cmd))
            {
                switch (cmd)
                {
                    case "amiadmin":
                        {
                            string result = player.IsAdmin ? "You are an administrator." : "You are not an administrator.";

                            CaptureTheFlagBot.SendPrivateMessage(player, result);
                        }
                        break;
                    case "coins":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                PlayerDatabaseRow row = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);

                                CaptureTheFlagBot.SendPrivateMessage(player, $"You currently have {row.Coins} coin{(row.Coins == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                    case "donatecoins":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                if (cmdTokens.Length >= 3)
                                {
                                    if (cmdTokens[1] != player.Username)
                                    {
                                        PlayerDatabaseRow playerToDonateData = PlayersDatabaseTable.GetPlayerDatabaseRow(cmdTokens[1]);

                                        if (playerToDonateData != null)
                                        {
                                            PlayerDatabaseRow donatorData = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);

                                            if (int.TryParse(cmdTokens[2], out int amount))
                                            {
                                                if (donatorData.Coins >= amount)
                                                {
                                                    playerToDonateData.Coins += amount;
                                                    donatorData.Coins -= amount;

                                                    CaptureTheFlagBot.SendPrivateMessage(player, $"Success! You donated {amount} coin{(amount == 1 ? "" : "s")} to player {cmdTokens[1].ToUpper()}.");
                                                }
                                                else
                                                {
                                                    CaptureTheFlagBot.SendPrivateMessage(player, "You don't have enough coins to donate that amount!");
                                                }
                                            }
                                            else
                                            {
                                                CaptureTheFlagBot.SendPrivateMessage(player, "Third parameter of command is invalid! It must be a number.");
                                            }
                                        }
                                        else
                                        {
                                            CaptureTheFlagBot.SendPrivateMessage(player, $"Player {cmdTokens[2].ToUpper()} does not exist.");
                                        }
                                    }
                                    else
                                    {
                                        CaptureTheFlagBot.SendPrivateMessage(player, "You can't donate coins to yourself!");
                                    }
                                }
                                else
                                {
                                    CaptureTheFlagBot.SendPrivateMessage(player, $"Insufficient amount of parameters for command.");
                                }
                            }
                        }
                        break;
                    case "help":
                        {
                            CaptureTheFlagBot.SendPrivateMessage(player, $"Command prefixes: . > ! #");

                            CaptureTheFlagBot.SendPrivateMessage(player, "Regular Commands:");
                            CaptureTheFlagBot.SendPrivateMessage(player, StringArrayToString(RegularCommands.ValidCommands));

                            CaptureTheFlagBot.SendPrivateMessage(player, "Game Commands:");
                            CaptureTheFlagBot.SendPrivateMessage(player, StringArrayToString(GameCommands.ValidCommands));

                            CaptureTheFlagBot.SendPrivateMessage(player, "Administrator Commands:");
                            CaptureTheFlagBot.SendPrivateMessage(player, StringArrayToString(AdminCommands.ValidCommands));

                            CaptureTheFlagBot.SendPrivateMessage(player, "Tips:");
                            CaptureTheFlagBot.SendPrivateMessage(player, $"- Press arrow keys/WASD keys around an enemy player to attack them.");
                            CaptureTheFlagBot.SendPrivateMessage(player, $"- Use nurse smiley to heal your teammates!");
                            CaptureTheFlagBot.SendPrivateMessage(player, $"- There is an item shop in the clouds.");
                            CaptureTheFlagBot.SendPrivateMessage(player, $"- Watch out for traps around the map.");
                        }
                        break;
                    case "totalwins":
                    case "totallosses":
                    case "losses":
                    case "wins":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                PlayerDatabaseRow row = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);
                                int resultCount = cmd == "totalwins" || cmd == "wins" ? row.TotalWins : row.TotalLosses;
                                string type = cmd == "totalwins" || cmd == "wins" ? "won" : "lost";

                                CaptureTheFlagBot.SendPrivateMessage(player, $"You have {type} {resultCount} time{(row.TotalWins == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                    case "totalkills":
                        {
                            if (PlayersDatabaseTable.Loaded)
                            {
                                PlayerDatabaseRow playerData = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);

                                CaptureTheFlagBot.SendPrivateMessage(player, $"You have killed a total of {playerData.TotalKills} player{(playerData.TotalKills == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                }
            }
            else
            {
                CaptureTheFlagBot.SendPrivateMessage(player, $"The command \"{cmd}\" is invalid!");
            }
        }

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