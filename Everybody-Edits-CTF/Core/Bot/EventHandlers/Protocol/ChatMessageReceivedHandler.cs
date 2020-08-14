// File Name:     ChatMessageReceivedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Commands;
using Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class ChatMessageReceivedHandler : BotEvent
    {
        /// <summary>
        /// The array of command objects. This array will always have a length of three.
        /// 
        /// Index Map:
        /// 0 - Admin commands
        /// 1 - Game commands
        /// 2 - Regular commands
        /// </summary>
        public static Command[] BotCommands = new Command[]
        {
            new AdminCommands(),
            new GameCommands(),
            new RegularCommands()
        };

        /// <summary>
        /// Event handler for when a chat message is received in the Everybody Edits world.
        /// </summary>
        public ChatMessageReceivedHandler() : base(new string[] { EverybodyEditsMessage.ChatMessage }, null)
        {

        }

        /// <summary>
        /// Handles a chat message being received in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            int playerId = message.GetInt(0);

            if (ctfBot.JoinedWorld.Players.ContainsKey(playerId))
            {
                // Handle bot commands (admin, game, and regular)
                HandleCommand(ctfBot, ctfBot.JoinedWorld.Players[playerId], message.GetString(1));
            }
        }

        /// <summary>
        /// Executes all <see cref="Command"/> objects defined in the <see cref="BotCommands"/> array. The objects are executed via the <see cref="Command.Handle(Player, ParsedCommand)"/>
        /// method. If the method returns true, that means the command was executed succesfully, thus resulting in the other items in the array being ignored.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="player">The player that is executing a command.</param>
        /// <param name="chatMessage">The chat message to be parsed.</param>
        private void HandleCommand(CtfBot ctfBot, Player player, string chatMessage)
        {
            ParsedCommand parsedCommand = Command.GetParsedCommand(chatMessage);

            if (parsedCommand != null)
            {
                foreach (Command command in BotCommands)
                {
                    if (command.Handle(ctfBot, player, parsedCommand)) // Break out of foreach loop if the command was succesfully handled
                    {
                        break;
                    }
                }
            }
        }
    }
}