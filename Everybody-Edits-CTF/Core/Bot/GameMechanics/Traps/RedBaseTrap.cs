// File Name:     RedBaseTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Traps
{
    public class RedBaseTrap : Trap
    {
        /// <summary>
        /// The locations where the red base trap can be triggered.
        /// </summary>
        public override Point[] TriggerLocations
        {
            get => new Point[]
            {
                new Point(361, 175)
            };
        }

        /// <summary>
        /// Handles the trap that is located in the red team's base. This trap can only be activated by the red team.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(Player player)
        {
            if (player.Team != Team.Red)
            {
                return;
            }

            if (PlayerOnTrapTrigger(player, TriggerLocations[0]) && !TrapActivated)
            {
                Task.Run(async() =>
                {
                    TrapActivated = true;

                    // Close gate
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 178), 1058);
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 179), 1058);

                    // Remove bridge
                    for (int x = 347; x <= 353; x++)
                    {
                        CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 180), 0);

                        await Task.Delay(100);
                    }

                    await Task.Delay(TrapCooldownMs);

                    // Show bridge
                    for (int x = 347; x <= 353; x++)
                    {
                        CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 180), 47);

                        await Task.Delay(100);
                    }

                    // Remove gate
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 178), 0);
                    CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(354, 179), 0);

                    await Task.Delay(TrapCooldownMs);

                    TrapActivated = false;
                });
            }
        }
    }
}