// File Name:     BridgeTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.GameMechanics.Traps
{
    public class BridgeTrap : Trap
    {
        /// <summary>
        /// The locations where the bridge trap can be triggered.
        /// </summary>
        public override Point[] TriggerLocations
        {
            get => new Point[]
            {
                new Point(89, 179),
                new Point(110, 179)
            };
        }

        /// <summary>
        /// Handles the bridge trap. This trap can be activated by both the blue team and the red team.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(Player player)
        {
            foreach (Point triggerLocation in TriggerLocations)
            {
                if (PlayerOnTrapTrigger(player, triggerLocation))
                {
                    if (!TrapActivated)
                    {
                        Task.Run(async() =>
                        {
                            TrapActivated = true;

                            for (int x = 94; x <= 105; x++)
                            {
                                CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 180), 0);

                                await Task.Delay(100);
                            }

                            await Task.Delay(TrapCooldownMs);

                            for (int x = 94; x <= 105; x++)
                            {
                                CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 180), 47);

                                await Task.Delay(100);
                            }

                            await Task.Delay(TrapCooldownMs);

                            TrapActivated = false;
                        });
                    }
                }
            }
        }
    }
}