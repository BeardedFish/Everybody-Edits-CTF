// File Name:     RoomEntrance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class RoomEntrance : IGameMechanic
    {
        private const int DoorBlockId = 1026;

        public void Handle(string messageType, Player player)
        {
            if (JoinedWorld.Blocks == null || !player.IsPlayingGame)
            {
                return;
            }

            if (player.Location.X > 0
                && player.HorizontalDirection == HorizontalDirection.Left
                && JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X - 1, player.Location.Y].Id == DoorBlockId)
            {
                CtfBot.TeleportPlayer(player, player.Location.X - 2, player.Location.Y);
            }
            else if (player.Location.X < JoinedWorld.Width &&
                player.HorizontalDirection == HorizontalDirection.Right
                && JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X + 1, player.Location.Y].Id == DoorBlockId)
            {
                CtfBot.TeleportPlayer(player, player.Location.X + 2, player.Location.Y);
            }
        }
    }
}