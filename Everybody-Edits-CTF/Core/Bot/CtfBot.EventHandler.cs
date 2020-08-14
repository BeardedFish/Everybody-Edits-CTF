// File Name:     CtfBot.EventHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, August 14, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot
{
    public partial class CtfBot
    {
        private readonly BotEvent[] botEventHandlers;

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

                Connect();
            }
        }

        /// <summary>
        /// Event handler for every time the bot receives a message from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="message">The message object that contains data about the message received.</param>
        private void OnMessage(object sender, Message message)
        {
            foreach (BotEvent eventHandler in botEventHandlers)
            {
                if (!eventHandler.Equals(message.Type))
                {
                    continue;
                }

                eventHandler.Handle(this, message);
            }
        }
    }
}