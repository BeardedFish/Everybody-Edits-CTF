// File Name:     ParsedCommand.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

namespace Everybody_Edits_CTF.Core.Bot.DataStructures
{
    public sealed class ParsedCommand
    {
        /// <summary>
        /// The prefix of the command.
        /// </summary>
        public char Prefix { get; set; }

        /// <summary>
        /// The string which contains the single word command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The parameters of the command. If no parameters, then
        /// </summary>
        public string[] Parameters { get; set; }

        /// <summary>
        /// Constructor for <see cref="ParsedCommand"/> object which stores data about a parsed command.
        /// </summary>
        /// <param name="prefix">Refer to <see cref="Prefix"/> for description.</param>
        /// <param name="command">Refer to <see cref="Command"/> for description.</param>
        /// <param name="parameters">Refer to <see cref="Parameters"/> for description.</param>
        public ParsedCommand(char prefix, string command, string[] parameters)
        {
            Prefix = prefix;
            Command = command;
            Parameters = parameters;
        }
    }
}