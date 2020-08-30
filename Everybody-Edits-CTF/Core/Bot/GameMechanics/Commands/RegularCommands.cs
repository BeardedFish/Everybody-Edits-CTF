// File Name:     RegularCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Database;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class RegularCommands : Command
    {
        public RegularCommands() : base(new string[] { "amiadmin", "coins", "donatecoins", "help", "spectate", "totalwins", "totallosses", "losses", "wins", "totalkills" })
        {

        }

        /// <summary>
        /// Handles a player executing a regular command. A regular command can be executed by any player.
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
                switch (parsedCommand.Command)
                {
                    case "amiadmin":
                        {
                            string result = MySqlDatabase.GetRow(player.Username).IsAdministrator ? "You are an administrator." : "You are not an administrator.";

                            ctfBot.SendPrivateMessage(player, result);
                        }
                        break;
                    case "coins":
                        {
                            if (MySqlDatabase.Loaded)
                            {
                                PlayerData row = MySqlDatabase.GetRow(player.Username);

                                ctfBot.SendPrivateMessage(player, $"You currently have {row.Statistics.Coins} coin{(row.Statistics.Coins == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                    case "donatecoins":
                        {
                            if (MySqlDatabase.Loaded)
                            {
                                if (parsedCommand.Parameters.Length >= 2)
                                {
                                    if (parsedCommand.Parameters[0] != player.Username)
                                    {
                                        PlayerData playerToDonateData = MySqlDatabase.GetRow(parsedCommand.Parameters[0]);

                                        if (playerToDonateData != null)
                                        {
                                            PlayerData donatorData = MySqlDatabase.GetRow(player.Username);

                                            if (int.TryParse(parsedCommand.Parameters[1], out int amount))
                                            {
                                                if (donatorData.Statistics.Coins >= amount)
                                                {
                                                    playerToDonateData.Statistics.Coins += amount;
                                                    donatorData.Statistics.Coins -= amount;

                                                    ctfBot.SendPrivateMessage(player, $"Success! You donated {amount} coin{(amount == 1 ? "" : "s")} to player {parsedCommand.Parameters[0].ToUpper()}.");
                                                }
                                                else
                                                {
                                                    ctfBot.SendPrivateMessage(player, "You don't have enough coins to donate that amount!");
                                                }
                                            }
                                            else
                                            {
                                                ctfBot.SendPrivateMessage(player, "Third parameter of command is invalid! It must be a number.");
                                            }
                                        }
                                        else
                                        {
                                            ctfBot.SendPrivateMessage(player, $"Player {parsedCommand.Parameters[0].ToUpper()} does not exist.");
                                        }
                                    }
                                    else
                                    {
                                        ctfBot.SendPrivateMessage(player, "You can't donate coins to yourself!");
                                    }
                                }
                                else
                                {
                                    ctfBot.SendPrivateMessage(player, "Insufficient amount of parameters for command.");
                                }
                            }
                        }
                        break;
                    case "help":
                        {
                            ctfBot.SendPrivateMessage(player, "Command prefixes: . > ! #");

                            ctfBot.SendPrivateMessage(player, "Regular Commands:");
                            ctfBot.SendPrivateMessage(player, StringArrayToString(ctfBot.BotCommands[2].ValidCommands));

                            ctfBot.SendPrivateMessage(player, "Game Commands:");
                            ctfBot.SendPrivateMessage(player, StringArrayToString(ctfBot.BotCommands[1].ValidCommands));

                            ctfBot.SendPrivateMessage(player, "Administrator Commands:");
                            ctfBot.SendPrivateMessage(player, StringArrayToString(ctfBot.BotCommands[0].ValidCommands));

                            ctfBot.SendPrivateMessage(player, "Tips:");
                            ctfBot.SendPrivateMessage(player, "- Press arrow keys/WASD keys around an enemy player to attack them.");
                            ctfBot.SendPrivateMessage(player, "- Use hard hat/worker smiley to dig dirt.");
                            ctfBot.SendPrivateMessage(player, "- Use doctor/nurse smiley to heal your teammates.");
                            ctfBot.SendPrivateMessage(player, "- There is an item shop in the clouds.");
                            ctfBot.SendPrivateMessage(player, "- Watch out for traps around the map.");
                        }
                        break;
                    case "spectate":
                        {
                            if (!player.IsPlayingGame && !player.CanToggleGodMode)
                            {
                                ctfBot.SetForceFly(player, !player.IsInGodMode);

                                if (player.IsInGodMode)
                                {
                                    player.GoToLobby(ctfBot);
                                }

                                ctfBot.SendPrivateMessage(player, player.IsInGodMode ? "You have left spectate mode." : $"You have entered spectate mode. Type {parsedCommand.Prefix}spectate again to exit out of spectate mode.");
                            }
                            else
                            {
                                string privateMessage = player.IsPlayingGame ? "This command is only available to players not playing!" : "You can toggle God mode! Use that instead.";

                                ctfBot.SendPrivateMessage(player, privateMessage);
                            }
                        }
                        break;
                    case "totalwins":
                    case "totallosses":
                    case "losses":
                    case "wins":
                        {
                            if (MySqlDatabase.Loaded)
                            {
                                PlayerData row = MySqlDatabase.GetRow(player.Username);
                                int resultCount = parsedCommand.Command == "totalwins" || parsedCommand.Command == "wins" ? row.Statistics.TotalWins : row.Statistics.TotalLosses;
                                string type = parsedCommand.Command == "totalwins" || parsedCommand.Command == "wins" ? "won" : "lost";

                                ctfBot.SendPrivateMessage(player, $"You have {type} {resultCount} time{(row.Statistics.TotalWins == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                    case "totalkills":
                        {
                            if (MySqlDatabase.Loaded)
                            {
                                PlayerData playerData = MySqlDatabase.GetRow(player.Username);

                                ctfBot.SendPrivateMessage(player, $"You have killed a total of {playerData.Statistics.TotalKills} player{(playerData.Statistics.TotalKills == 1 ? "" : "s")}.");
                            }
                        }
                        break;
                }
            }
            else
            {
                ctfBot.SendPrivateMessage(player, $"The command \"{parsedCommand.Command}\" is invalid!");
            }

            return canHandle;
        }

        /// <summary>
        /// Converts a string array to a single string. Each string item in the string array (excluding the last item) is seperated by a comma and a space.
        /// </summary>
        /// <param name="array">The string array to be converted to a single string.</param>
        /// <returns>A <see cref="string"/> which contains the data of the string array seperated by commas.</returns>
        private string StringArrayToString(string[] array)
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