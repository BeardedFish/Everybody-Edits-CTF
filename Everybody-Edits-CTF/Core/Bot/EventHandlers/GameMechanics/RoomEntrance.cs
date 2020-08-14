// File Name:     RoomEntrance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class RoomEntrance : IGameMechanic
    {
        /// <summary>
        /// The Everybody Edits block id that represents a door a player can teleport through.
        /// </summary>
        private const int DoorBlockId = 1026;

        /// <summary>
        /// Handles all room entrance doors in the Everybody Edits world. A room entrance door is defined in the <see cref="DoorBlockId"/> variable. If a player taps
        /// left or right on that block, they are telported to the opposite side of it.
        /// </summary>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(string messageType, Player player)
        {
            if (JoinedWorld.Blocks == null || !player.IsPlayingGame)
            {
                return;
            }

            if (player.Location.X > 0
                && player.HorizontalDirection == HorizontalDirection.Left
                && JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X - 1, player.Location.Y].Id == DoorBlockId) // Teleport to left
            {
                CtfBot.TeleportPlayer(player, player.Location.X - 2, player.Location.Y);
            }
            else if (player.Location.X < JoinedWorld.Width &&
                player.HorizontalDirection == HorizontalDirection.Right
                && JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X + 1, player.Location.Y].Id == DoorBlockId) // Teleport to right
            {
                CtfBot.TeleportPlayer(player, player.Location.X + 2, player.Location.Y);
            }
        }
    }
}