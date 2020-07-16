// File Name:     LavaLakeTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.GameMechanics.Traps
{
    public class LavaLakeTrap : Trap
    {
        /// <summary>
        /// The locations where the lava lake trap can be triggered.
        /// </summary>
        public override Point[] TriggerLocations
        {
            get => new Point[]
            {
                new Point(277, 179)
            };
        }

        /// <summary>
        /// Handles the trap that is located in the lava lake. This trap can be activated by both the blue team and the red team, however, the player must be wearing the Fire Demon
        /// smiley.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(Player player)
        {
            if (PlayerOnTrapTrigger(player, TriggerLocations[0]) && player.SmileyId == (int)Smiley.FireDemon)
            {
                if (!TrapActivated)
                {
                    Task.Run(async() =>
                    {
                        CaptureTheFlagBot.SendPrivateMessage(player, "You triggered a secret trap!");

                        TrapActivated = true;

                        for (int y = 176; y >= 172; y--)
                        {
                            CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(259, y), 1058);
                            CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(306, y), 1058);

                            await Task.Delay(100);
                        }

                        await Task.Delay(TrapCooldownMs);

                        for (int y = 172; y <= 176; y++)
                        {
                            CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(259, y), 0);
                            CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, new Point(306, y), 0);

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