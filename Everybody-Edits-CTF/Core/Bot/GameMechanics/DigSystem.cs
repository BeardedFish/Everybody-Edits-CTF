﻿// File Name:     DigSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 20, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class DigSystem : DelayedAction
    {
        /// <summary>
        /// The delay in milliseconds a player must wait in order to dig in the Everybody Edits world.
        /// </summary>
        private const int DigDelayMs = 750;

        /// <summary>
        /// The block id's that can be dug in the Everybody Edits world.
        /// </summary>
        private readonly int[] diggableBlocks = { 16, 21, 1045, 142, 179, 180, 181 };

        /// <summary>
        /// Handles a player digging dirt blocks in the Everybody Edits world. All diggable blocks are defined in the <see cref="diggableBlocks"/> integer array, where the
        /// integer represents a block id. A player can only dig blocks if they are wearing a specific smiley. Refer to the <see cref="WearingDiggableSmiley(Player)"/>
        /// method for a list of smilies that can dig. A player must tap the WASD or arrow keys in order to dig.
        /// </summary>
        /// <param name="player">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public DigSystem(CaptureTheFlagBot ctfBot) : base(ctfBot, DigDelayMs)
        {

        }

        /// <summary>
        /// Handles a player digging in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (ctfBot.JoinedWorld.Blocks == null || !player.IsPlayingGame || player.IsInGodMode || player.IsPressingSpacebar || !WearingDiggableSmiley(player))
            {
                return;
            }

            if (IsDelayOver(player))
            {
                bool handled;

                foreach (int blockId in diggableBlocks)
                {
                    if (handled = CanDigLeft(ctfBot, player, blockId))
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X - 1, player.Location.Y), Blocks.Foreground.SlowGravityDot);
                    }
                    else if (handled = CanDigRight(ctfBot, player, blockId))
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X + 1, player.Location.Y), Blocks.Foreground.SlowGravityDot);
                    }
                    else if (handled = CanDigUp(ctfBot, player, blockId))
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X, player.Location.Y - 1), Blocks.Foreground.SlowGravityDot);
                    }
                    else if (handled = CanDigDown(ctfBot, player, blockId))
                    {
                        ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X, player.Location.Y + 1), Blocks.Foreground.SlowGravityDot);
                    }

                    if (handled) // Player digged a block successfully
                    {
                        UpdatePlayerCurrentTick(player);

                        break; // No need to check the other blocks
                    }
                }
            }
        }

        /// <summary>
        /// Returns a boolean that states whether a player is wearing a smiley that can dig or not.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>True if the player is wearing either the <see cref="Smiley.HardHat"/> or the <see cref="Smiley.Worker"/>, if not, false.</returns>
        public static bool WearingDiggableSmiley(Player player)
        {
            return player.SmileyId == (int)Smiley.HardHat || player.SmileyId == (int)Smiley.Worker;
        }

        /// <summary>
        /// States whether a player can dig left in the Everybody Edits world or not.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be checked whether they can dig left or not.</param>
        /// <param name="blockId">The diggable block id.</param>
        /// <returns>True if the player can dig left, if not, false.</returns>
        private static bool CanDigLeft(CaptureTheFlagBot ctfBot, Player player, int blockId)
        {
            return player.HorizontalDirection == HorizontalDirection.Left
                && player.Location.X > 0
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X - 1, player.Location.Y].Id == blockId;
        }

        /// <summary>
        /// States whether a player can dig right in the Everybody Edits world or not.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be checked whether they can dig right or not.</param>
        /// <param name="blockId">The diggable block id.</param>
        /// <returns>True if the player can dig right, if not, false.</returns>
        private static bool CanDigRight(CaptureTheFlagBot ctfBot, Player player, int blockId)
        {
            return player.HorizontalDirection == HorizontalDirection.Right
                && player.Location.X < ctfBot.JoinedWorld.Width
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X + 1, player.Location.Y].Id == blockId;
        }

        /// <summary>
        /// States whether a player can dig up in the Everybody Edits world or not.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be checked whether they can dig up or not.</param>
        /// <param name="blockId">The diggable block id.</param>
        /// <returns>True if the player can dig up, if not, false.</returns>
        private static bool CanDigUp(CaptureTheFlagBot ctfBot, Player player, int blockId)
        {
            return player.VerticalDirection == VerticalDirection.Up
                && player.Location.Y > 0
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y - 1].Id == blockId;
        }

        /// <summary>
        /// States whether a player can dig down in the Everybody Edits world or not.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be checked whether they can dig down or not.</param>
        /// <param name="blockId">The diggable block id.</param>
        /// <returns>True if the player can dig down, if not, false.</returns>
        private static bool CanDigDown(CaptureTheFlagBot ctfBot, Player player, int blockId)
        {
            return player.VerticalDirection == VerticalDirection.Down
                && player.Location.Y < ctfBot.JoinedWorld.Height
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y + 1].Id == blockId;
        }
    }
}