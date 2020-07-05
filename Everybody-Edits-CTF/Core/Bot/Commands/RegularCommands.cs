// File Name:     RegularCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, July 5, 2020

using Everybody_Edits_CTF.Core.Data;
using Everybody_Edits_CTF.Core.DataStructures;

namespace Everybody_Edits_CTF.Core.Bot.Commands
{
    public static class RegularCommands
    {
        /// <summary>
        /// Handles a regular command that an Everybody Edits player wants to execute.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        /// <param name="cmd">The command that the player wants to execute.</param>
        /// <param name="cmdTokens">The command tokens (strings seperated by a space character).</param>
        public static void Handle(Player player, string cmd, string[] cmdTokens)
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
                        CaptureTheFlagBot.SendPrivateMessage(player, $"Commands: amiadmin, disconnect, heal, health, help, lobby, scores, suicide, blueflag, redflag");
                        CaptureTheFlagBot.SendPrivateMessage(player, $"Use nurse smiley to heal your teammates!");
                        CaptureTheFlagBot.SendPrivateMessage(player, $"Bot is still work in progress so some things might be glitchy/not work.");
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
                default:
                    {
                        CaptureTheFlagBot.SendPrivateMessage(player, $"The command \"{cmd}\" is invalid!");
                    }
                    break;
            }
        }
    }
}