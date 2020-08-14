// File Name:     BotEventHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot
{
    public class CtfBotEventHandler
    {
        private readonly BotEvent[] botEventHandlers = new BotEvent[]
        {
            new InitHandler(),
            new BlockPlacedHandler(),
            new ChatMessageReceivedHandler(),
            new WorldActionHandler(),
            new GodModeToggledHandler(),
            new PlayerJoinedWorldHandler(),
            new PlayerLocationChangedHandler(),
            new PlayerResetHandler(),
            new PlayerTeamChangedHandler(),
            new SmileyChangedHandler()
        };

        /// <summary>
        /// Handles all messages that a Capture the Flag bot receives.
        /// </summary>
        /// <param name="connection">The connection of the bot to Everybody Edits.</param>
        public CtfBotEventHandler(Connection connection)
        {
            connection.OnDisconnect += OnDisconnect;
            connection.OnMessage += OnMessage;
        }

        /// <summary>
        /// Event handler for when the bot is disconnected from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="message">The reason why the bot was disconnected.</param>
        private void OnDisconnect(object sender, string message)
        {
            JoinedWorld.Players.Clear();

            PlayersDatabaseTable.Save();
            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Disconnected from the Everybody Edits world (Reason: {message}).");
            
            // Only reconnect if the bot was not disconnected on purpose
            if (BotSettings.AutoReconnectOnDisconnect && message != "Disconnect")
            {
                Logger.WriteLog(LogType.EverybodyEditsMessage, "Auto reconnecting...");

                CtfBot.Connect();
            }
        }

        /// <summary>
        /// Event handler for every time the bot receives a message from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="m">The message object that contains data about the message received.</param>
        private void OnMessage(object sender, Message m)
        {
            foreach (BotEvent eventHandler in botEventHandlers)
            {
                if (!eventHandler.Equals(m.Type))
                {
                    continue;
                }

                eventHandler.Handle(m);
            }
        }
    }
}