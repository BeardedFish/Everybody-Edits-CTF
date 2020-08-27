// File Name:     AdminCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Database;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class AdminCommands : Command
    {
        public AdminCommands() : base(new string[] { "ban", "disconnect", "kick" })
        {

        }

        /// <summary>
        /// Handles a player executing an administrator command. Administrator players are defined in the MySql database.
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
                if (PlayersTable.GetRow(player.Username).IsAdministrator)
                {
                    switch (parsedCommand.Command)
                    {
                        case "ban":
                            {
                                // TODO: Make it so that players that don't exist in the database are first inserted and then banned...

                                if (PlayersTable.Loaded)
                                {
                                    if (parsedCommand.Parameters.Length != 1)
                                    {
                                        string username = parsedCommand.Parameters[0];
                                        PlayersTableRow playerData = PlayersTable.GetRow(username);

                                        if (playerData != null)
                                        {
                                            if (!playerData.IsBanned)
                                            {
                                                ctfBot?.KickPlayer(username, $"You've been banned from this world.");
                                                playerData.IsBanned = true;
                                            }
                                            else
                                            {
                                                ctfBot?.SendPrivateMessage(player, $"Player {username.ToUpper()} is already banned!");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ctfBot?.SendPrivateMessage(player, "Invalid amount of parameters! Total required: 1.");
                                    }
                                }
                                else
                                {
                                    ctfBot?.SendPrivateMessage(player, "This command is disabled due to the database not being loaded!");
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
                    }
                }
                else
                {
                    ctfBot.SendPrivateMessage(player, "You don't have permission to execute this command.");
                }
            }

            return canHandle;
        }
    }
}