// File Name:     Rooms.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, June 30, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Enums;
using System.Collections.Generic;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class RoomEntrances
    {
        private static readonly Dictionary<Point, Point> ShopEntrances = new Dictionary<Point, Point>()
        {
            { new Point(228, 110), new Point(230, 110) },
            { new Point(246, 110), new Point(248, 110) }
        };

        private static readonly Dictionary<Point, Point> EmptyBuildingEntrances = new Dictionary<Point, Point>()
        {
            { new Point(45, 110), new Point(47, 110) },
            { new Point(78, 108), new Point(80, 108) },
            { new Point(91, 108), new Point(93, 108) },
            { new Point(98, 110), new Point(100, 110) },
            { new Point(104, 110), new Point(106, 110) },
            { new Point(129, 110), new Point(131, 110) },
            { new Point(149, 110), new Point(151, 110) },
            { new Point(311, 109), new Point(313, 109) }
        };

        public static void Handle(Player player)
        {
            if (player.IsInGodMode)
            {
                return;
            }

            HandleWarpPipes(player);
            HandleShop(player);
            HandleEmptyBuildings(player);
        }

        private static void HandleWarpPipes(Player player)
        {
            if (player.VerticalDirection == VerticalDirection.Down)
            {
                // Warp pipe #1
                if (player.Location == new Point(60, 170))
                {
                    CaptureTheFlagBot.TeleportPlayer(player, 60, 174);
                }

                // Warp pipe #2
                if (player.Location == new Point(128, 175))
                {
                    CaptureTheFlagBot.TeleportPlayer(player, 128, 179);
                }
            }
        }

        private static void HandleShop(Player player)
        {
            HandleHorizontalEntrances(player, ShopEntrances);
        }

        private static void HandleEmptyBuildings(Player player)
        {
            HandleHorizontalEntrances(player, EmptyBuildingEntrances);
        }

        private static void HandleHorizontalEntrances(Player player, Dictionary<Point, Point> entrances)
        {
            Point leftEntrance;
            Point rightEntrance;

            foreach (KeyValuePair<Point, Point> entrance in entrances)
            {
                leftEntrance = entrance.Key;
                rightEntrance = entrance.Value;

                if (player.HorizontalDirection == HorizontalDirection.Left && player.Location == rightEntrance)
                {
                    CaptureTheFlagBot.TeleportPlayer(player, leftEntrance.X, leftEntrance.Y);

                    break;
                }

                if (player.HorizontalDirection == HorizontalDirection.Right && player.Location == leftEntrance)
                {
                    CaptureTheFlagBot.TeleportPlayer(player, rightEntrance.X, rightEntrance.Y);

                    break;
                }
            }
        }
    }
}