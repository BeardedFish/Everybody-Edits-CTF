// File Name:     WarpPipe.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class WarpPipe : IGameMechanic
    {
        /// <summary>
        /// The Everybody Edits block id that represents a warp pipe entrance that a player can enter when their vertical direction is <see cref="VerticalDirection.Down"/>.
        /// </summary>
        private const int WarpPipeEntranceBlockId = 1055;

        /// <summary>
        /// Handles all warp pipes in the Everybody Edits. If a player presses down on the block id <see cref="WarpPipeEntranceBlockId"/>, they they are teleported down
        /// one block.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(CtfBot ctfBot, string messageType, Player player)
        {
            if (ctfBot.JoinedWorld.Blocks == null || !player.IsPlayingGame)
            {
                return;
            }

            if (player.Location.Y < ctfBot.JoinedWorld.Height
                && player.VerticalDirection == VerticalDirection.Down
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y + 1].Id == WarpPipeEntranceBlockId)
            {
                ctfBot.TeleportPlayer(player, player.Location.X, player.Location.Y + 1);
            }
        }
    }
}