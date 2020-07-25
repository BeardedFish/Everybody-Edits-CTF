// File Name:     RoomEntrances.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, June 30, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class RoomEntrances
    {
        /// <summary>
        /// The time a player has to wait in order to enter a room entrance door, in milliseconds.
        /// </summary>
        private const long EntranceCooldownMs = 1000;

        /// <summary>
        /// List of locations of warp pipes in the Everybody Edits world. The first point in the entrance point and the second point is the location that the warp pipe
        /// leads the player to.
        /// </summary>
        private static readonly (Point, Point)[] WarpPipes = new (Point, Point)[]
        {
            (new Point(60, 170), new Point(60, 174)),
            (new Point(128, 175), new Point(128, 179))
        };

        /// <summary>
        /// List of locations that can make a player enter/leave the shop in the Everybody Edits world. The first point is the entrance that leads to the second point and the
        /// second point is the entrance that leads to the first point.
        /// </summary>
        private static readonly (Point, Point)[] ShopEntrances = new (Point, Point)[]
        {
            (new Point(228, 110), new Point(230, 110)),
            (new Point(246, 110), new Point(248, 110))
        };

        /// <summary>
        /// A dictionary that keeps track of a Player's last room entrance time tick, in milliseconds.
        /// </summary>
        private static Dictionary<Player, long> lastEntranceTick = new Dictionary<Player, long>();

        /// <summary>
        /// List of locations that can make a player enter/leave the empty buildings in the Everybody Edits world. The first point is the entrance that leads to the second point
        /// and the second point is the entrance that leads to the first point.
        /// </summary>
        private static readonly (Point, Point)[] EmptyBuildingEntrances = new (Point, Point)[]
        {
            (new Point(45, 110), new Point(47, 110)),
            (new Point(78, 108), new Point(80, 108)),
            (new Point(91, 108), new Point(93, 108)),
            (new Point(98, 110), new Point(100, 110)),
            (new Point(104, 110), new Point(106, 110)),
            (new Point(129, 110), new Point(131, 110)),
            (new Point(149, 110), new Point(151, 110)),
            (new Point(311, 109), new Point(313, 109))
        };

        /// <summary>
        /// Handles a player trying to pass through an entrance in the Everybody Edits world. These entrances do not work if the player is in god mode.
        /// </summary>
        /// <param name="player">The player to handle.</param>
        public static void Handle(Player player)
        {
            if (player.IsInGodMode || !player.IsPlayingGame || player.IsPressingSpacebar)
            {
                return;
            }

            if (!lastEntranceTick.ContainsKey(player))
            {
                lastEntranceTick.Add(player, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }

            // Only allow the player to enter the entrance if they waited the cooldown time
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastEntranceTick[player] >= EntranceCooldownMs)
            {
                if (HandleWarpPipes(player) || HandleShop(player) || HandleEmptyBuildings(player)) // Succesful entrance, update the last time time
                {
                    lastEntranceTick[player] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
            }
        }

        /// <summary>
        /// Handles any warp pipes in the Everybody Edits world. If a player presses down on a warp pipe, they are telported inside of it. Warp pipe entrance locations are defined
        /// in <see cref="WarpPipes"/>.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        /// <returns>True if the player succesfully entered a warp pipe, if not, false.</returns>
        private static bool HandleWarpPipes(Player player)
        {
            if (player.VerticalDirection == VerticalDirection.Down)
            {
                // NOTE: Item1 is the warp pipe entrance and Item2 is the location that the warp pipe leads too

                foreach ((Point, Point) warpPipe in WarpPipes)
                {
                    if (player.Location == warpPipe.Item1)
                    {
                        CaptureTheFlagBot.TeleportPlayer(player, warpPipe.Item2.X, warpPipe.Item2.Y);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Handles all shop entrances in the Everybody Edits world. Empty entrance locations are defined in <see cref="ShopEntrances"/>.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        /// <returns>True if the player succesfully enters/leaves the shop, if not, false.</returns>
        private static bool HandleShop(Player player)
        {
            return HandleHorizontalEntrances(player, ShopEntrances);
        }

        /// <summary>
        /// Handles all empty building entrances in the Everybody Edits world. Empty entrance locations are defined in <see cref="EmptyBuildingEntrances"/>.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        /// <returns>True if the player succesfully enters/leaves the empty buildings, if not, false.</returns>
        private static bool HandleEmptyBuildings(Player player)
        {
            return HandleHorizontalEntrances(player, EmptyBuildingEntrances);
        }

        /// <summary>
        /// Handles entrances where a player can move horizontally by pressing the A and D keys or the left and right keys. If a player is located at an entrance point, then
        /// they are teleported to the other side of the entrance.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        /// <param name="entrances">The array of Point pairs that hold all the possible entrances that the player can go through.</param>
        /// <returns>True if the player succesfully enters/leaves one of the entances defined in the entrances parameter, if not, false.</returns>
        private static bool HandleHorizontalEntrances(Player player, (Point, Point)[] entrances)
        {
            Point leftEntrance;
            Point rightEntrance;

            foreach ((Point, Point) entrance in entrances)
            {
                leftEntrance = entrance.Item1;
                rightEntrance = entrance.Item2;

                if (player.HorizontalDirection == HorizontalDirection.Left && player.Location == rightEntrance) // Player is at the right entrance pressing left
                {
                    CaptureTheFlagBot.TeleportPlayer(player, leftEntrance.X, leftEntrance.Y);

                    return true;
                }

                if (player.HorizontalDirection == HorizontalDirection.Right && player.Location == leftEntrance) // Player is at the left entrance pressing right
                {
                    CaptureTheFlagBot.TeleportPlayer(player, rightEntrance.X, rightEntrance.Y);

                    return true;
                }
            }

            return false;
        }
    }
}