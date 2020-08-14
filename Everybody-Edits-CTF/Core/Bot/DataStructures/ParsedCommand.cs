// File Name:     Commands.cs
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
        /// The command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The parameters of the command. If no parameters, then
        /// </summary>
        public string[] Parameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ParsedCommand(char prefix, string command, string[] parameters)
        {
            Prefix = prefix;
            Command = command;
            Parameters = parameters;
        }
    }
}
