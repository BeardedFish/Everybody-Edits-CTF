// File Name:     BridgeTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Traps
{
    public class BridgeTrap : Trap
    {
        /// <summary>
        /// Bridge trap that has two trigger points. 
        /// </summary>
        public BridgeTrap() : base(new Point[] { new Point(89, 179), new Point(110, 179) })
        {

        }

        /// <summary>
        /// Handles the bridge trap. This trap can be activated by both the blue team and the red team.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!base.CanTriggerTrap(player))
            {
                return;
            }

            if (!TrapActivated)
            {
                Task.Run(async() =>
                {
                    TrapActivated = true;

                    // Remove bridge
                    for (int x = 94; x <= 105; x++)
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 180), Blocks.None);

                        await Task.Delay(100);
                    }

                    await Task.Delay(TrapCooldownMs);

                    // Place bridge
                    for (int x = 94; x <= 105; x++)
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(x, 180), Blocks.Foreground.FactoryWood);

                        await Task.Delay(100);
                    }

                    await Task.Delay(TrapCooldownMs);

                    TrapActivated = false;
                });
            }
        }
    }
}