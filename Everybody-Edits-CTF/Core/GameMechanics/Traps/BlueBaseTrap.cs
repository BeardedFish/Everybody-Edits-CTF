// File Name:     BlueBaseTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.GameMechanics.Traps
{
    public class BlueBaseTrap : Trap
    {
        /// <summary>
        /// The locations where the blue base trap can be triggered.
        /// </summary>
        public override Point[] TriggerLocations
        {
            get => new Point[]
            {
                new Point(38, 175)
            };
        }

        /// <summary>
        /// Handles the trap that is located in the blue team's base. This trap can only be activated by the red team.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(Player player)
        {
            if (player.Team != Team.Blue)
            {
                return;
            }

            if (PlayerOnTrapTrigger(player, TriggerLocations[0]) && !TrapActivated)
            {
                Task.Run(async() =>
                {
                    TrapActivated = true;

                    // Close gate
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 178), 1058);
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 179), 1058);

                    // Pour lava
                    for (int i = 175; i <= 179; i++)
                    {
                        CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(47, i), 416);
                        CaptureTheFlagBot.PlaceBlock(BlockLayer.Background, new Point(47, i), 629);

                        await Task.Delay(100);
                    }

                    await Task.Delay(TrapCooldownMs);

                    // Remove lava
                    for (int i = 175; i <= 179; i++)
                    {
                        CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(47, i), 0);
                        CaptureTheFlagBot.PlaceBlock(BlockLayer.Background, new Point(47, i), 507);

                        await Task.Delay(100);
                    }

                    // Open gate
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 178), 0);
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(45, 179), 0);

                    await Task.Delay(TrapCooldownMs);

                    TrapActivated = false;
                });
            }
        }
    }
}