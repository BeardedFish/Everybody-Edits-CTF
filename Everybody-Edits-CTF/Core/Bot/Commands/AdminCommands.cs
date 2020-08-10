// File Name:     AdminCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, July 5, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;

namespace Everybody_Edits_CTF.Core.Bot.Commands
{
    public static class AdminCommands
    {
        /// <summary>
        /// An array of type <see cref="string"/> which contains the valid administrator commands.
        /// </summary>
        public static readonly string[] ValidCommands =
        {
            "disconnect",
            "kick"
        };

        /// <summary>
        /// Handles an admin command that an Everybody Edits player wants to execute.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        /// <param name="cmd">The command that the player wants to execute.</param>
        /// <param name="cmdTokens">The command tokens (strings seperated by a space character).</param>
        public static void Handle(Player player, string cmd, string[] cmdTokens)
        {
            if (player.IsAdmin)
            {
                switch (cmd)
                {
                    case "disconnect":
                        {
                            CaptureTheFlagBot.Disconnect();
                        }
                        break;
                    case "kick":
                        {
                            if (player.IsAdmin)
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
                                    CaptureTheFlagBot.SendPrivateMessage(player, $"Insufficient amount of parameters for command.");
                                }
                            }
                            else
                            {
                                CaptureTheFlagBot.SendPrivateMessage(player, $"You don't have permission to execute this command.");
                            }
                        }
                        break;
                }
            }
            else
            {
                CaptureTheFlagBot.SendPrivateMessage(player, $"You don't have permission to execute this command.");
            }
        }
    }
}