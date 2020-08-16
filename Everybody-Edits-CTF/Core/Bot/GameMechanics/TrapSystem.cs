// File Name:     TrapSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.GameMechanics.Traps;
using System;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class TrapSystem
    {
        /// <summary>
        /// The traps in the Everybody Edits world that can be triggered by players playing the Capture The Flag game.
        /// </summary>
        private static readonly Trap[] traps = new Trap[]
        {
            new BlueBaseTrap(),
            new BridgeTrap(),
            new LavaLakeTrap(),
            new RedBaseTrap()
        };

        /// <summary>
        /// Handles all traps in the Everybody Edits worlds. Traps can only be triggered when certain conditions are met. Refer to <see cref="Trap.CanTriggerTrap(Player)"/>
        /// for more information.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            foreach (Trap trap in traps)
            {
                if (!IsPlayerOnTrapTrigger(player, trap))
                {
                    continue;
                }

                trap.Handle(ctfBot, player);
            }
        }

        /// <summary>
        /// States whether a player is on a trap trigger or not.
        /// </summary>
        /// <param name="player">The player to check if they are on the trap or not.</param>
        /// <param name="trap">The trap to check.</param>
        /// <returns>True if the player is on the trap trigger, if not, false.</returns>
        private static bool IsPlayerOnTrapTrigger(Player player, Trap trap)
        {
            foreach (Point triggerLocation in trap.TriggerLocations)
            {
                if (player.Location.Y == triggerLocation.Y)
                {
                    int x = Math.Abs(player.Location.X - triggerLocation.X);
                    int trapLocationExtraBlocks = 1;

                    if (x <= trapLocationExtraBlocks)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}