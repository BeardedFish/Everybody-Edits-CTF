// File Name:     Trap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Traps
{
    public abstract class Trap
    {
        /// <summary>
        /// The amount of time players have to wait in order to reactivate a trap, in milliseconds.
        /// </summary>
        public const int TrapCooldownMs = 2500;

        /// <summary>
        /// The trigger locations of the trap in the Everybody Edits world.
        /// </summary>
        public readonly Point[] TriggerLocations;
        
        /// <summary>
        /// Constructor for creating a <see cref="Trap"/> object which contains information about a trap in the Everybody Edits world.
        /// </summary>
        /// <param name="triggerLocations">The trigger locations for the trap.</param>
        public Trap(Point[] triggerLocations)
        {
            TriggerLocations = triggerLocations;
        }

        /// <summary>
        /// States whether the trap has been activated or not.
        /// </summary>
        public bool TrapActivated { get; protected set; }

        /// <summary>
        /// Handles a trap in the Everybody Edits worlds. Implementation will vary.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="player">The player to be handled.</param>
        public abstract void Handle(CtfBot ctfBot, Player player);

        /// <summary>
        /// States whether a player can trigger this trap or not.
        /// 
        /// By default, the condition to trigger a trap is:
        ///     - The trap must not be currently activated.
        ///     - The player must be playing the Capture The Flag game (aka: team must be blue or red).
        ///     - The players vertical direction must be equal to <see cref="VerticalDirection.Down"/>.
        /// </summary>
        /// <param name="player">The player to check if they can trigger this trap or not.</param>
        /// <returns>True if the player can trigger this trap, if not, false.</returns>
        public virtual bool CanTriggerTrap(Player player)
        {
            return !TrapActivated && player.IsPlayingGame && player.VerticalDirection == VerticalDirection.Down;
        }
    }
}