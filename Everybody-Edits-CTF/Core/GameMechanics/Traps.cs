// File Name:     Traps.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.Bot;
using System.Drawing;
using System.Threading;
using System;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class Traps
    {
        private const int TrapCooldownMs = 2500;

        private static readonly Point BlueTeamTrapLocation = new Point(38, 175);
        private static readonly Point RedTeamTrapLocation = new Point(361, 175);
        private static readonly Point[] BridgeTrapLocation = { new Point(89, 179), new Point(110, 179) };
        private static readonly Point SecretTrapLocation = new Point(277, 179);

        private static bool BlueBaseTrapActivated, RedBaseTrapActivated, BridgeTrapActivated, SecretTrapActivated;

        /// <summary>
        /// Handles all traps in the Everybody Edits world.
        /// </summary>
        /// <param name="player">The player that is triggering the traps.</param>
        public static void Handle(Player player)
        {
            if (player.IsInGodMode || player.VerticalDirection != VerticalDirection.Down)
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
        /// <param name="player">The player that is triggering the traps.</param>
        private static void HandleBlueTeamTraps(Player player)
        {
            if (player.Team != Team.Blue)
            {
                return;
            }

            if (PlayerOnTrapTrigger(player, BlueTeamTrapLocation) && !BlueBaseTrapActivated)
            {
                new Thread(delegate()
                {
                    Thread.CurrentThread.IsBackground = true;

                    BlueBaseTrapActivated = true;

                    // Close gate
                    CaptureTheFlagBot.PlaceBlock(0, 45, 178, 1058);
                    CaptureTheFlagBot.PlaceBlock(0, 45, 179, 1058);

                    // Pour lava
                    for (int i = 175; i <= 179; i++)
                    {
                        CaptureTheFlagBot.PlaceBlock(0, 47, i, 416);
                        CaptureTheFlagBot.PlaceBlock(1, 47, i, 629);

                        Thread.Sleep(100);
                    }

                    Thread.Sleep(TrapCooldownMs);

                    // Remove lava
                    for (int i = 175; i <= 179; i++)
                    {
                        CaptureTheFlagBot.PlaceBlock(0, 47, i, 0);
                        CaptureTheFlagBot.PlaceBlock(1, 47, i, 507);

                        Thread.Sleep(100);
                    }

                    // Open gate
                    CaptureTheFlagBot.PlaceBlock(0, 45, 178, 0);
                    CaptureTheFlagBot.PlaceBlock(0, 45, 179, 0);

                    Thread.Sleep(TrapCooldownMs);

                    BlueBaseTrapActivated = false;
                }).Start();
            }
        }

        /// <summary>
        /// Handles all traps that the red team can trigger.
        /// </summary>
        /// <param name="player">The player that is triggering the traps.</param>
        private static void HandleRedTeamTraps(Player player)
        {
            if (player.Team != Team.Red)
            {
                return;
            }

            if (PlayerOnTrapTrigger(player, RedTeamTrapLocation) && !RedBaseTrapActivated)
            {
                new Thread(delegate()
                {
                    Thread.CurrentThread.IsBackground = true;

                    RedBaseTrapActivated = true;

                    // Close gate
                    CaptureTheFlagBot.PlaceBlock(0, 354, 178, 1058);
                    CaptureTheFlagBot.PlaceBlock(0, 354, 179, 1058);

                    // Remove bridge
                    for (int x = 347; x <= 353; x++)
                    {
                        CaptureTheFlagBot.PlaceBlock(0, x, 180, 0);

                        Thread.Sleep(100);
                    }

                    Thread.Sleep(TrapCooldownMs);

                    // Show bridge
                    for (int x = 347; x <= 353; x++)
                    {
                        CaptureTheFlagBot.PlaceBlock(0, x, 180, 47);

                        Thread.Sleep(100);
                    }

                    // Remove gate
                    CaptureTheFlagBot.PlaceBlock(0, 354, 178, 0);
                    CaptureTheFlagBot.PlaceBlock(0, 354, 179, 0);

                    Thread.Sleep(TrapCooldownMs);

                    RedBaseTrapActivated = false;
                }).Start();
            }
        }

        /// <summary>
        /// Handles all traps that both the blue team and the red team can trigger.
        /// </summary>
        /// <param name="player">The player that is triggering the traps.</param>
        private static void HandleBridgeTraps(Player player)
        {
            // Trap for both teams
            if (PlayerOnTrapTrigger(player, BridgeTrapLocation[0]) || PlayerOnTrapTrigger(player, BridgeTrapLocation[1]))
            {
                if (player.Team == Team.Blue || player.Team == Team.Red)
                {
                    if (!BridgeTrapActivated)
                    {
                        new Thread(delegate()
                        {
                            Thread.CurrentThread.IsBackground = true;

                            BridgeTrapActivated = true;

                            for (int x = 94; x <= 105; x++)
                            {
                                CaptureTheFlagBot.PlaceBlock(0, x, 180, 0);

                                Thread.Sleep(100);
                            }

                            Thread.Sleep(TrapCooldownMs);

                            for (int x = 94; x <= 105; x++)
                            {
                                CaptureTheFlagBot.PlaceBlock(0, x, 180, 47);

                                Thread.Sleep(100);
                            }

                            Thread.Sleep(TrapCooldownMs);

                            BridgeTrapActivated = false;
                        }).Start();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void HandleSecretTrap(Player player)
        {
            if (PlayerOnTrapTrigger(player, SecretTrapLocation))
            {
                if (!SecretTrapActivated)
                {
                    new Thread(delegate()
                    {
                        Thread.CurrentThread.IsBackground = true;

                        CaptureTheFlagBot.SendPrivateMessage(player, "You trigerred a secret trap!");

                        SecretTrapActivated = true;

                        for (int y = 176; y >= 172; y--)
                        {
                            CaptureTheFlagBot.PlaceBlock(0, 259, y, 1058);
                            CaptureTheFlagBot.PlaceBlock(0, 306, y, 1058);

                            Thread.Sleep(100);
                        }

                        Thread.Sleep(TrapCooldownMs);

                        for (int y = 172; y <= 176; y++)
                        {
                            CaptureTheFlagBot.PlaceBlock(0, 259, y, 0);
                            CaptureTheFlagBot.PlaceBlock(0, 306, y, 0);

                            Thread.Sleep(100);
                        }

                        Thread.Sleep(TrapCooldownMs);

                        SecretTrapActivated = false;
                    }).Start();
                }
            }
        }

        /// <summary>
        /// States whether a player is on a trap trigger or not.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="trapLocation"></param>
        /// <returns>True if the player is on the trap trigger, if not, false.</returns>
        private static bool PlayerOnTrapTrigger(Player player, Point trapLocation)
        {
            if (player.Location.Y == trapLocation.Y)
            {
                int x = Math.Abs(player.Location.X - trapLocation.X);

                if (x <= 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}