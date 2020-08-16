// File Name:     RoomEntrance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class RoomEntrance
    {
        /// <summary>
        /// The Everybody Edits block id that represents a door a player can teleport through.
        /// </summary>
        private const int DoorBlockId = 1026;

        /// <summary>
        /// Handles all room entrance doors in the Everybody Edits world. A room entrance door is defined in the <see cref="DoorBlockId"/> variable. If a player presses
        /// left or right on that block, they are telported to the opposite side of it.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (ctfBot.JoinedWorld.Blocks == null || !player.IsPlayingGame || player.IsInGodMode || player.IsPressingSpacebar)
            {
                return;
            }

            if (player.Location.X > 0
                && player.HorizontalDirection == HorizontalDirection.Left
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X - 1, player.Location.Y].Id == DoorBlockId) // Teleport to left
            {
                ctfBot.TeleportPlayer(player, player.Location.X - 2, player.Location.Y);
            }
            else if (player.Location.X < ctfBot.JoinedWorld.Width &&
                player.HorizontalDirection == HorizontalDirection.Right
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X + 1, player.Location.Y].Id == DoorBlockId) // Teleport to right
            {
                ctfBot.TeleportPlayer(player, player.Location.X + 2, player.Location.Y);
            }
        }
    }
}