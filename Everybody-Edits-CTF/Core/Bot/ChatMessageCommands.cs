// File Name:     ChatMessageCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, July 4, 2020

using Everybody_Edits_CTF.Core.Bot.Commands;
using Everybody_Edits_CTF.Core.DataStructures;
using System;

namespace Everybody_Edits_CTF.Core.Bot
{
    public static class ChatMessageCommands
    {
        /// <summary>
        /// List of characters that an Everybody Edits message must start with in order to be considered a bot command.
        /// </summary>
        private static readonly char[] CommandPrefixes = { '.', '>', '!', '#' };

        /// <summary>
        /// Handles all bot commands (admin, game, and regular).
        /// </summary>
        /// <param name="player">The player that sent the chat message.</param>
        /// <param name="chatMessage">The chat message that the player said.</param>
        public static void Handle(Player player, string chatMessage)
        {
            string[] cmdTokens = chatMessage.Split(' ');

            if (IsBotCommand(cmdTokens[0]))
            {
                string cmd = cmdTokens[0].Substring(1, cmdTokens[0].Length - 1).ToLower();

                if (IsAdminCommand(cmd))
                {
                    AdminCommands.Handle(player, cmd, cmdTokens);
                }
                else if (IsGameCommand(cmd))
                {
                    GameCommands.Handle(player, cmd, cmdTokens);
                }
                else
                {
                    RegularCommands.Handle(player, cmd, cmdTokens);
                }
            }
        }

        /// <summary>
        /// States whether a chat message is a valid bot command or not. A valid bot command has a prefix of any value defined in <see cref="CommandPrefixes"/>.
        /// </summary>
        /// <param name="chatMessage">The chat message to be evaluated.</param>
        /// <returns>True if the string is a valid bot command, if not, false.</returns>
        private static bool IsBotCommand(string chatMessage)
        {
            if (chatMessage.Length >= 2)
            {
                for (int i = 0; i < CommandPrefixes.Length; i++)
                {
                    if (chatMessage[0] == CommandPrefixes[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Compares a string (command) to an array of valid commands to see whether it is valid or not.
        /// </summary>
        /// <param name="cmd">The command to be compared to the array of strings.</param>
        /// <param name="validCmds">The array of strings that contains all the valid commands.</param>
        /// <returns>True if the command is equal to a value in the string array, if not, false.</returns>
        private static bool IsValidCommand(string cmd, string[] validCmds)
        {
            for (int i = 0; i < validCmds.Length; i++)
            {
                if (string.Equals(cmd, validCmds[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// States whether a command is a valid admin command or not. A valid admin command is defined in <see cref="AdminCommands.ValidCommands"/>.
        /// </summary>
        /// <param name="cmd">The command to be evaluated.</param>
        /// <returns>True if the command is a valid admin command, if not, false.</returns>
        private static bool IsAdminCommand(string cmd)
        {
            return IsValidCommand(cmd, AdminCommands.ValidCommands);
        }

        /// <summary>
        /// States whether a command is a valid game command or not. A valid game command is defined in <see cref="GameCommands.ValidCommands"/>.
        /// </summary>
        /// <param name="cmd">The command to be evaluated.</param>
        /// <returns>True if the command is a valid game command, if not, false.</returns>
        private static bool IsGameCommand(string cmd)
        {
            return IsValidCommand(cmd, GameCommands.ValidCommands);
        }
    }
}