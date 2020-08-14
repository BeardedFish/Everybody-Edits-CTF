﻿// File Name:     ChatMessageReceivedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Commands;
using Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class ChatMessageReceivedHandler : BotEvent
    {
        /// <summary>
        /// The array of command objects. This object is initialized in the <see cref="ChatMessageReceivedHandler()"/> constructor.
        /// </summary>
        private Command[] botCommands;

        /// <summary>
        /// Event handler for when a chat message is received in the Everybody Edits world.
        /// </summary>
        public ChatMessageReceivedHandler() : base(new string[] { EverybodyEditsMessage.ChatMessage }, null)
        {
            botCommands = new Command[]
            {
                new AdminCommands(),
                new GameCommands(),
                new RegularCommands()
            };
        }

        /// <summary>
        /// Handles a chat message being received in the Everybody Edits world.
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
            int playerId = message.GetInt(0);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                // Handle bot commands (admin, game, and regular)
                HandleCommand(JoinedWorld.Players[playerId], message.GetString(1));
            }
        }

        /// <summary>
        /// Executes all <see cref="Command"/> objects defined in the <see cref="botCommands"/> array. The objects are executed via the <see cref="Command.Handle(Player, ParsedCommand)"/>
        /// method. If the method returns true, that means the command was executed succesfully, thus resulting in the other items in the array being ignored.
        /// </summary>
        /// <param name="player">The player that is executing a command.</param>
        /// <param name="chatMessage">The chat message to be parsed.</param>
        private void HandleCommand(Player player, string chatMessage)
        {
            ParsedCommand parsedCommand = Command.GetParsedCommand(chatMessage);

            if (parsedCommand != null)
            {
                foreach (Command command in botCommands)
                {
                    if (command.Handle(player, parsedCommand)) // Break out of foreach loop if the command was succesfully handled
                    {
                        break;
                    }
                }
            }
        }
    }
}