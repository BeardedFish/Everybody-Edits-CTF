// File Name:     BlueBaseTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Traps
{
    public class BlueBaseTrap : Trap
    {
        /// <summary>
        /// Trap that is located in the blue base.
        /// </summary>
        public BlueBaseTrap() : base(new Point[] { new Point(38, 175) })
        {

        }

        /// <summary>
        /// Handles the trap that is located in the blue teams base. This trap can only be activated by the blue team.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(CtfBot ctfBot, Player player)
        {
            if (!CanTriggerTrap(player))
            {
                return;
            }

            Task.Run(async() =>
            {
                TrapActivated = true;

                // Close gate
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 178), 1058);
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 179), 1058);

                // Pour lava
                for (int i = 175; i <= 179; i++)
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(47, i), 416);
                    ctfBot.PlaceBlock(BlockLayer.Background, new Point(47, i), 629);

                    await Task.Delay(100);
                }

                await Task.Delay(TrapCooldownMs);

                // Remove lava
                for (int i = 175; i <= 179; i++)
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(47, i), 0);
                    ctfBot.PlaceBlock(BlockLayer.Background, new Point(47, i), 507);

                    await Task.Delay(100);
                }

                // Open gate
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 178), 0);
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 179), 0);

                await Task.Delay(TrapCooldownMs);

                TrapActivated = false;
            });
        }

        /// <summary>
        /// States whether a player can trigger this trap or not.
        /// 
        /// The condition to trigger this trap is:
        ///     - The base implementation is met.
        ///     - The players team is blue.
        /// </summary>
        /// <param name="player">The player to check if they can trigger this trap or not.</param>
        /// <returns>True if the player can trigger this trap, if not, false.</returns>
        public override bool CanTriggerTrap(Player player)
        {
            return base.CanTriggerTrap(player) && player.Team == Team.Blue;
        }
    }
}