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
        /// The amount of time players have to wait in order to reactivate a trap.
        /// </summary>
        public const int TrapCooldownMs = 2500;

        /// <summary>
        /// The trigger locations of the trap in the Everybody Edits world.
        /// </summary>
        public readonly Point[] TriggerLocations;
        
        public Trap(Point[] triggerLocations)
        {
            TriggerLocations = triggerLocations;
        }

        /// <summary>
        /// States whether the trap has been activated or not.
        /// </summary>
        public bool TrapActivated { get; protected set; }

        public abstract void Handle(Player player);

        public virtual bool CanTriggerTrap(Player player)
        {
            return !TrapActivated && player.IsPlayingGame && player.VerticalDirection == VerticalDirection.Down;
        }
    }
}