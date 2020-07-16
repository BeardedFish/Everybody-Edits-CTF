// File Name:     Traps.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using System.Drawing;
using System;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class Traps
    {
        /// <summary>
        /// The amount of time a player has to wait to trigger a trap if it has been triggered.
        /// </summary>
        private const int TrapCooldownMs = 2500;

        /// <summary>
        /// The location of the trap that can only be triggered by the blue team.
        /// </summary>
        private static readonly Point BlueTeamTrapLocation = new Point(38, 175);

        /// <summary>
        /// The location of the trap that can only be triggered by the red team.
        /// </summary>
        private static readonly Point RedTeamTrapLocation = new Point(361, 175);

        /// <summary>
        /// The location of the bridge trap that can be triggered by both the blue team and the red team.
        /// </summary>
        private static readonly Point[] BridgeTrapLocation = { new Point(89, 179), new Point(110, 179) };

        /// <summary>
        /// The location of the secret trap (aka: trap in the lava lake) that can be triggered by players wearing the Fire Demon smiley.
        /// </summary>
        private static readonly Point SecretTrapLocation = new Point(277, 179);

        /// <summary>
        /// States whether the trap has been activated by a player or not.
        /// </summary>
        private static bool BlueBaseTrapActivated, RedBaseTrapActivated, BridgeTrapActivated, SecretTrapActivated;

        /// <summary>
        /// Handles all traps in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player that is triggering a trap in the Everybody Edits world.</param>
        public static void Handle(Player player)
        {
            if (player.IsInGodMode || !player.IsPlayingGame || player.VerticalDirection != VerticalDirection.Down)
            {
                return;
            }

            HandleBlueTeamTraps(player);
            HandleRedTeamTraps(player);
            HandleBridgeTraps(player);
            HandleSecretTrap(player);
        }

        /// <summary>
        /// Handles all traps that the blue team can trigger.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        private static void HandleBlueTeamTraps(Player player)
        {
            if (player.Team != Team.Blue)
            {
                return;
            }

            if (PlayerOnTrapTrigger(player, BlueTeamTrapLocation) && !BlueBaseTrapActivated)
            {
                Task.Run(async() =>
                {
                    BlueBaseTrapActivated = true;

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

                    BlueBaseTrapActivated = false;
                });
            }
        }

        /// <summary>
        /// Handles all traps that the red team can trigger.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        private static void HandleRedTeamTraps(Player player)
        {
            if (player.Team != Team.Red)
            {
                return;
            }

            if (PlayerOnTrapTrigger(player, RedTeamTrapLocation) && !RedBaseTrapActivated)
            {
                Task.Run(async() =>
                {
                    RedBaseTrapActivated = true;

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

                    RedBaseTrapActivated = false;
                });
            }
        }

        /// <summary>
        /// Handles the bridge trap that both the blue team and the red team can trigger.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        private static void HandleBridgeTraps(Player player)
        {
            // Trap for both teams
            if (PlayerOnTrapTrigger(player, BridgeTrapLocation[0]) || PlayerOnTrapTrigger(player, BridgeTrapLocation[1]))
            {
                if (!BridgeTrapActivated)
                {
                    Task.Run(async() =>
                    {
                        BridgeTrapActivated = true;

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

                        BridgeTrapActivated = false;
                    });
                }
            }
        }

        /// <summary>
        /// Handles the secret trap located in the lava lake. This trap can be used by players wearing the Fire Demon smiley.
        /// </summary>
        /// <param name="player">The player that is triggering the trap.</param>
        public static void HandleSecretTrap(Player player)
        {
            if (PlayerOnTrapTrigger(player, SecretTrapLocation) && player.SmileyId == (int)Smiley.FireDemon)
            {
                if (!SecretTrapActivated)
                {
                    Task.Run(async() =>
                    {
                        CaptureTheFlagBot.SendPrivateMessage(player, "You triggered a secret trap!");

                        SecretTrapActivated = true;

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

                        SecretTrapActivated = false;
                    });
                }
            }
        }

        /// <summary>
        /// States whether a player is on a trap trigger or not.
        /// </summary>
        /// <param name="player">The player to check if they are on the trap or not.</param>
        /// <param name="trapLocation">The middle location point of the trap trigger.</param>
        /// <returns>True if the player is on the trap trigger, if not, false.</returns>
        private static bool PlayerOnTrapTrigger(Player player, Point trapLocation)
        {
            if (player.Location.Y == trapLocation.Y)
            {
                int x = Math.Abs(player.Location.X - trapLocation.X);
                int trapLocationExtraBlocks = 1;

                if (x <= trapLocationExtraBlocks)
                {
                    return true;
                }
            }

            return false;
        }
    }
}