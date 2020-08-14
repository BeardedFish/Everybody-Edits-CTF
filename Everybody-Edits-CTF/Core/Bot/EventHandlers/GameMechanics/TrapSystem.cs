// File Name:     TrapSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Traps;
using System;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class TrapSystem : IGameMechanic
    {
        private readonly Trap[] traps;

        public TrapSystem()
        {
            traps = new Trap[]
            {
                new BlueBaseTrap(),
                new BridgeTrap(),
                new LavaLakeTrap(),
                new RedBaseTrap()
            };
        }

        public void Handle(string messageType, Player player)
        {
            foreach (Trap trap in traps)
            {
                if (!IsPlayerOnTrapTrigger(player, trap))
                {
                    continue;
                }

                trap.Handle(player);
            }
        }

        /// <summary>
        /// States whether a player is on a trap trigger or not.
        /// </summary>
        /// <param name="player">The player to check if they are on the trap or not.</param>
        /// <param name="trap">TODO: Write...</param>
        /// <returns>True if the player is on the trap trigger, if not, false.</returns>
        private bool IsPlayerOnTrapTrigger(Player player, Trap trap)
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