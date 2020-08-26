// File Name:     WarpPipe.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class WarpPipe : DelayedAction
    {
        /// <summary>
        /// The cooldown time in milliseconds a player must wait before they can enter a warp pipe again.
        /// </summary>
        private const int WarpPipeCooldownMs = 1500;

        /// <summary>
        /// The Everybody Edits block id that represents a warp pipe entrance that a player can enter when their vertical direction is <see cref="VerticalDirection.Down"/>.
        /// </summary>
        private const int WarpPipeEntranceBlockId = 1055;

        /// <summary>
        /// Game mechanic which handles the warp pipes in the Capture The Flag game.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public WarpPipe(CaptureTheFlagBot ctfBot) : base(ctfBot, WarpPipeCooldownMs)
        {
            ctfBot.OnPlayerMoved += OnPlayerMoved;
        }

        /// <summary>
        /// Handles all warp pipes in the Everybody Edits. If a player presses down on the block id <see cref="WarpPipeEntranceBlockId"/>, they they are teleported down
        /// one block.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnPlayerMoved(CaptureTheFlagBot ctfBot, Player player)
        {
            if (ctfBot.JoinedWorld.Blocks == null || !player.IsPlayingGame)
            {
                return;
            }

            if (IsDelayOver(player))
            {
                if (player.Location.Y < ctfBot.JoinedWorld.Height
                    && player.VerticalDirection == VerticalDirection.Down
                    && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y + 1].Id == WarpPipeEntranceBlockId)
                {
                    ctfBot.TeleportPlayer(player, player.Location.X, player.Location.Y + 1);
                }

                UpdatePlayerCurrentTick(player);
            }
        }
    }
}