// File Name:     EverybodyEditsBotEvent.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, July 19, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;
using System;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers
{
    public abstract class BotEvent
    {
        /// <summary>
        /// 
        /// </summary>
        protected IGameMechanic[] GameMechanics { get; private set; }

        /// <summary>
        /// The <see cref="Message.Type"/> that will trigger this bot event.
        /// </summary>
        protected string[] TriggerMessages { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triggerMessage"></param>
        public BotEvent(string[] triggerMessages, IGameMechanic[] gameMechanics)
        {
            TriggerMessages = triggerMessages;
            GameMechanics = gameMechanics;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Handle(Message message)
        {
            if (!TriggerMessages.Contains(message.Type))
            {
                throw new Exception("The message type is incompatible with this bot event class.");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="triggerMessage"></param>
        /// <param name="player"></param>
        public virtual void ExecuteGameMechanics(string triggerMessage, Player player)
        {
            if (GameMechanics == null)
            {
                return;
            }

            foreach (IGameMechanic mechanic in GameMechanics)
            {
                mechanic?.Handle(triggerMessage, player);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triggerMessage"></param>
        /// <returns></returns>
        public bool Equals(string triggerMessage)
        {
            return TriggerMessages.Contains(triggerMessage);
        }
    }
}