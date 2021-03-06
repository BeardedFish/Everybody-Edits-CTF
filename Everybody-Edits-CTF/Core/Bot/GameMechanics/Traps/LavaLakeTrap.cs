﻿// File Name:     LavaLakeTrap.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Traps
{
    public class LavaLakeTrap : Trap
    {
        public LavaLakeTrap() : base(new Point[] { new Point(277, 136) })
        {

        }

        /// <summary>
        /// Handles the trap that is located in the lava lake. This trap can be activated by both the blue team and the red team, however, the player must be wearing the Fire Demon
        /// smiley.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that is triggering the trap.</param>
        public override void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!CanTriggerTrap(player))
            {
                return;
            }

            if (!TrapActivated)
            {
                Task.Run(async() =>
                {
                    ctfBot.SendPrivateMessage(player, "You triggered a secret trap!");

                    TrapActivated = true;

                    // Place wall
                    for (int y = 133; y >= 129; y--)
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(259, y), Blocks.Foreground.Caution);
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(306, y), Blocks.Foreground.Caution);

                        await Task.Delay(100);
                    }

                    await Task.Delay(TrapCooldownMs);

                    // Remove wall
                    for (int y = 129; y <= 133; y++)
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(259, y), Blocks.None);
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(306, y), Blocks.None);

                        await Task.Delay(100);
                    }

                    await Task.Delay(TrapCooldownMs);

                    TrapActivated = false;
                });
            }
        }

        /// <summary>
        /// <para>
        /// States whether a player can trigger this trap or not.
        /// </para>
        /// <para>
        /// The condition to trigger this trap is:
        /// <br>- The base implementation is met.</br>
        /// <br>- The player is wearing the Fire Demon smiley.</br>
        /// </para>
        /// </summary>
        /// <param name="player">The player to check if they can trigger this trap or not.</param>
        /// <returns>True if the player can trigger this trap, if not, false.</returns>
        public override bool CanTriggerTrap(Player player)
        {
            return base.CanTriggerTrap(player) && player.SmileyId == (int)Smiley.FireDemon;
        }
    }
}