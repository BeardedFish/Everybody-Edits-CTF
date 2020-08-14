// File Name:     BlueBaseTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Traps
{
    public class BlueBaseTrap : Trap
    {
        public BlueBaseTrap() : base(new Point[] { new Point(38, 175) })
        {

        }

        /// <summary>
        /// Handles the trap that is located in the blue team's base. This trap can only be activated by the red team.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(Player player)
        {
            if (!CanTriggerTrap(player))
            {
                return;
            }

            Task.Run(async() =>
            {
                TrapActivated = true;

                // Close gate
                CtfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 178), 1058);
                CtfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 179), 1058);

                // Pour lava
                for (int i = 175; i <= 179; i++)
                {
                    CtfBot.PlaceBlock(BlockLayer.Foreground, new Point(47, i), 416);
                    CtfBot.PlaceBlock(BlockLayer.Background, new Point(47, i), 629);

                    await Task.Delay(100);
                }

                await Task.Delay(TrapCooldownMs);

                // Remove lava
                for (int i = 175; i <= 179; i++)
                {
                    CtfBot.PlaceBlock(BlockLayer.Foreground, new Point(47, i), 0);
                    CtfBot.PlaceBlock(BlockLayer.Background, new Point(47, i), 507);

                    await Task.Delay(100);
                }

                // Open gate
                CtfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 178), 0);
                CtfBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 179), 0);

                await Task.Delay(TrapCooldownMs);

                TrapActivated = false;
            });
        }

        public override bool CanTriggerTrap(Player player)
        {
            return base.CanTriggerTrap(player) && player.Team == Team.Blue;
        }
    }
}