// File Name:     RedBaseTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Traps
{
    public class RedBaseTrap : Trap
    {
        public RedBaseTrap() : base(new Point[] { new Point(361, 132) })
        {

        }

        /// <summary>
        /// Handles the trap that is located in the red teams base. This trap can only be activated by the red team.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!CanTriggerTrap(player))
            {
                return;
            }

            Task.Run(async() =>
            {
                TrapActivated = true;

                // Close gate
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 135), Blocks.Foreground.Caution);
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 136), Blocks.Foreground.Caution);

                // Remove bridge
                for (int x = 347; x <= 353; x++)
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 137), Blocks.None);

                    await Task.Delay(100);
                }

                await Task.Delay(TrapCooldownMs);

                // Show bridge
                for (int x = 347; x <= 353; x++)
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 137), Blocks.Foreground.FactoryWood);

                    await Task.Delay(100);
                }

                // Remove gate
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 135), Blocks.None);
                ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 136), Blocks.None);

                await Task.Delay(TrapCooldownMs);

                TrapActivated = false;
            });
        }

        /// <summary>
        /// <para>
        /// States whether a player can trigger this trap or not.
        /// </para>
        /// <para>
        /// The condition to trigger this trap is:
        /// <br>- The base implementation is met.</br>
        /// <br>- The players team is red.</br>
        /// </para>
        /// </summary>
        /// <param name="player">The player to check if they can trigger this trap or not.</param>
        /// <returns>True if the player can trigger this trap, if not, false.</returns>
        public override bool CanTriggerTrap(Player player)
        {
            return base.CanTriggerTrap(player) && player.Team == Team.Red;
        }
    }
}