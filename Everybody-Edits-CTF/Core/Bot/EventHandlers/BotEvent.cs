// File Name:     BotEvent.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

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
        /// The game mechanics that can be executed via the <see cref="ExecuteGameMechanics()"/>.
        /// </summary>
        protected IGameMechanic[] GameMechanics { get; private set; }

        /// <summary>
        /// The <see cref="Message.Type"/> that will trigger this bot event.
        /// </summary>
        protected string[] TriggerMessages { get; private set; }

        /// <summary>
        /// Constructor for creating a <see cref="BotEvent"/> object which is used for triggering an array of <see cref="IGameMechanic"/> when a certain <see cref="Message.Type"/>
        /// is received.
        /// </summary>
        /// <param name="triggerMessages">The array that contains all the strings that can trigger this bot event class.</param>
        /// <param name="gameMechanics">The array of game mechanics that can be executed via this class..</param>
        public BotEvent(string[] triggerMessages, IGameMechanic[] gameMechanics)
        {
            TriggerMessages = triggerMessages;
            GameMechanics = gameMechanics;
        }

        /// <summary>
        /// Abstract method which is supposed to handle a <see cref="Message.Type"/>. Implementation will vary.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        public abstract void Handle(Message message);

        /// <summary>
        /// Executes all game mechanics in the <see cref="GameMechanics"/> that are not null. The game mechanics are executed via the <see cref="IGameMechanic.Handle(string, Player)"/>
        /// method.
        /// </summary>
        /// <param name="messageType">The <see cref="Message.Type"/> that is triggering the game mechanic.</param>
        /// <param name="player">The player that the game mechanic(s) is executing for.</param>
        public virtual void ExecuteGameMechanics(string messageType, Player player)
        {
            if (GameMechanics != null)
            {
                foreach (IGameMechanic mechanic in GameMechanics)
                {
                    mechanic?.Handle(messageType, player);
                }
            }
        }

        /// <summary>
        /// States whether a string is equal to at least one item in the the <see cref="TriggerMessages"/>.
        /// </summary>
        /// <param name="triggerMessage">The string to be searched for in the string array.</param>
        /// <returns>True if the <see cref="TriggerMessages"/> contains the string, if not, false.</returns>
        public bool Equals(string triggerMessage)
        {
            return TriggerMessages.Contains(triggerMessage);
        }
    }
}