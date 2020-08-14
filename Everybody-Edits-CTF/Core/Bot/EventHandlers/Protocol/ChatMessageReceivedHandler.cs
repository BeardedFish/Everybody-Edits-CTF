// File Name:     ChatMessageReceivedHandler.cs
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
        private Command[] botCommands;

        public ChatMessageReceivedHandler() : base(new string[] { EverybodyEditsMessage.ChatMessage }, null)
        {
            botCommands = new Command[]
            {
                new AdminCommands(),
                new GameCommands(),
                new RegularCommands()
            };
        }

        public override void Handle(Message message)
        {
            base.Handle(message);

            int playerId = message.GetInt(0);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                HandleCommand(JoinedWorld.Players[playerId], message.GetString(1));
            }
        }

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