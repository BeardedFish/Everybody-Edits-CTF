// File Name:     RoomEntrance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class RoomEntrance : DelayedAction
    {
        /// <summary>
        /// The cooldown time in milliseconds a player must wait before they can use a door entrance again.
        /// </summary>
        private const int EntranceCooldownMs = 1000;

        /// <summary>
        /// The Everybody Edits block id that represents a door a player can teleport through.
        /// </summary>
        private const int DoorBlockId = 1026;

        /// <summary>
        /// Game mechanic which handles all room entrance doors in the Capture The Flag game. A room entrance door is defined in the <see cref="DoorBlockId"/> variable. If
        /// a player presses left or right on that block, they are telported to the opposite side of it.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public RoomEntrance(CaptureTheFlagBot ctfBot) : base(ctfBot, EntranceCooldownMs)
        {
            ctfBot.OnPlayerMoved += OnPlayerMoved;
        }

        /// <summary>
        /// Handles a player if they tap left or right on a block with the id of <see cref="DoorBlockId"/>.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnPlayerMoved(CaptureTheFlagBot ctfBot, Player player)
        {
            if (ctfBot.JoinedWorld.Blocks == null || !player.IsPlayingGame || player.IsInGodMode || player.IsPressingSpacebar)
            {
                return;
            }

            if (IsDelayOver(player))
            {
                bool handled;

                if (handled = (player.Location.X > 0
                    && player.HorizontalDirection == HorizontalDirection.Left
                    && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X - 1, player.Location.Y].Id == DoorBlockId)) // Teleport to left
                {
                    ctfBot.TeleportPlayer(player, player.Location.X - 2, player.Location.Y);
                }
                else if (handled = (player.Location.X < ctfBot.JoinedWorld.Width &&
                    player.HorizontalDirection == HorizontalDirection.Right
                    && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X + 1, player.Location.Y].Id == DoorBlockId)) // Teleport to right
                {
                    ctfBot.TeleportPlayer(player, player.Location.X + 2, player.Location.Y);
                }

                if (handled) // Player successfully was teleported
                {
                    UpdatePlayerCurrentTick(player);
                }
            }
        }
    }
}