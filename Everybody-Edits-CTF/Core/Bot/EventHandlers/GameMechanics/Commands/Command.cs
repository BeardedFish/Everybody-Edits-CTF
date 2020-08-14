﻿// File Name:     Commands.cs
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
        /// The array of strings that are valid commands. Values will vary depending on implementation.
        /// </summary>
        public readonly string[] ValidCommands;

        /// <summary>
        /// Constructor for creating a <see cref="Command"/> object which is used for creating commands for the Capture The Flag bot..
        /// </summary>
        /// <param name="validCommands">The string array of valid commands.</param>
        public Command(string[] validCommands)
        {
            ValidCommands = validCommands;
        }

        /// <summary>
        /// Handles a player executing a bot command. Implementation will vary.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="player">The player to be handled.</param>
        /// <param name="parsedCommand">The command to be handled.</param>
        /// <returns>Implementation will vary.</returns>
        public abstract bool Handle(CtfBot ctfBot, Player player, ParsedCommand parsedCommand);

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
        /// Parses an Everybody Edits chat message to a <see cref="ParsedCommand"/> object. The chat message must meet the <see cref="IsBotCommand(string)"/> condition
        /// in order to be parsed.
        /// </summary>
        /// <param name="message">The chat message to be parsed.</param>
        /// <returns>A <see cref="ParsedCommand"/> object if the chat message was succesfully parsed, if not, null.</returns>
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