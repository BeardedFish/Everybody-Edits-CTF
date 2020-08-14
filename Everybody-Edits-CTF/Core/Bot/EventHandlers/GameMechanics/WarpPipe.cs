// File Name:     WarpPipe.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class WarpPipe : IGameMechanic
    {
        private const int WarpPipeEntranceBlockId = 1055;

        public void Handle(string messageType, Player player)
        {
            if (JoinedWorld.Blocks == null || !player.IsPlayingGame)
            {
                return;
            }

            if (player.Location.Y < JoinedWorld.Height
                && player.VerticalDirection == VerticalDirection.Down
                && JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y + 1].Id == WarpPipeEntranceBlockId)
            {
                CtfBot.TeleportPlayer(player, player.Location.X, player.Location.Y + 1);
            }
        }
    }
}