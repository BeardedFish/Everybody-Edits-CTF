// File Name:     Trap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

using Everybody_Edits_CTF.Core.DataStructures;
using System;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.GameMechanics.Traps
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
        public abstract Point[] TriggerLocations { get; }
        
        /// <summary>
        /// States whether the trap has been activated or not.
        /// </summary>
        public bool TrapActivated { get; protected set; }

        /// <summary>
        /// Handles the trap. Implementation will depend on the type of trap.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public abstract void Handle(Player player);

        /// <summary>
        /// States whether a player is on a trap trigger or not.
        /// </summary>
        /// <param name="player">The player to check if they are on the trap or not.</param>
        /// <param name="trapLocation">The middle location point of the trap trigger.</param>
        /// <returns>True if the player is on the trap trigger, if not, false.</returns>
        public static bool PlayerOnTrapTrigger(Player player, Point trapLocation)
        {
            if (player.Location.Y == trapLocation.Y)
            {
                int x = Math.Abs(player.Location.X - trapLocation.X);
                int trapLocationExtraBlocks = 1;

                if (x <= trapLocationExtraBlocks)
                {
                    return true;
                }
            }

            return false;
        }
    }
}