// File Name:     Commands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Commands
{
    public abstract class Command
    {
        /// <summary>
        /// List of characters that an Everybody Edits message must start with in order to be considered a bot command.
        /// </summary>
        public static readonly char[] CommandPrefixes = { '.', '>', '!', '#' };

        /// <summary>
        /// 
        /// </summary>
        public readonly string[] ValidCommands;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validCommands"></param>
        public Command(string[] validCommands)
        {
            ValidCommands = validCommands;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="parsedCommand"></param>
        /// <returns></returns>
        public abstract bool Handle(Player player, ParsedCommand parsedCommand);

        /// <summary>
        /// States whether a chat message is a valid bot command or not. A valid bot command has a prefix of any value defined in <see cref="CommandPrefixes"/>.
        /// </summary>
        /// <param name="chatMessage">The chat message to be evaluated.</param>
        /// <returns>True if the string is a valid bot command, if not, false.</returns>
        private static bool IsBotCommand(string chatMessage)
        {
            if (chatMessage.Length >= 2)
            {
                foreach (char prefix in CommandPrefixes)
                {
                    if (chatMessage[0] == prefix)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ParsedCommand GetParsedCommand(string message)
        {
            if (!IsBotCommand(message))
            {
                return null;
            }

            string[] tokens = message.Substring(1, message.Length - 1).Split(' ');
            string[] parameters = tokens.Skip(1).Take(tokens.Length - 1).ToArray();

            return new ParsedCommand(message[0], tokens[0], parameters);
        }
    }
}