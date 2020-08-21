// File Name:     DigSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 20, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public class DigSystem : DelayedAction
    {
        /// <summary>
        /// 
        /// </summary>
        private const int DigDelayMs = 750;

        /// <summary>
        /// 
        /// </summary>
        public DigSystem() : base(DigDelayMs)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctfBot"></param>
        /// <param name="player"></param>
        public void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (ctfBot.JoinedWorld.Blocks == null || !player.IsPlayingGame || player.IsInGodMode || player.IsPressingSpacebar || !WearingDiggableSmiley(player))
            {
                return;
            }

            // Add new players to the tick dictionary
            if (!LastPlayerTickMs.ContainsKey(player))
            {
                UpdatePlayerCurrentTick(player);
            }

            if (IsDelayOver(player))
            {
                bool handled = false;

                if (CanDigLeft(ctfBot, player))
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X - 1, player.Location.Y), Blocks.Foreground.SlowGravityDot);

                    handled = true;
                }
                else if (CanDigRight(ctfBot, player))
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X + 1, player.Location.Y), Blocks.Foreground.SlowGravityDot);

                    handled = true;
                }
                else if (CanDigUp(ctfBot, player))
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X, player.Location.Y - 1), Blocks.Foreground.SlowGravityDot);

                    handled = true;
                }
                else if (CanDigDown(ctfBot, player))
                {
                    ctfBot.PlaceBlock(BlockLayer.Foreground, new Point(player.Location.X, player.Location.Y + 1), Blocks.Foreground.SlowGravityDot);

                    handled = true;
                }

                if (handled)
                {
                    UpdatePlayerCurrentTick(player);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool WearingDiggableSmiley(Player player)
        {
            return player.SmileyId == (int)Smiley.HardHat || player.SmileyId == (int)Smiley.Worker;
        }

        /// <summary>
        /// States whether a player can dig left or not.
        /// </summary>
        /// <param name="ctfBot"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private static bool CanDigLeft(CaptureTheFlagBot ctfBot, Player player)
        {
            return player.HorizontalDirection == HorizontalDirection.Left
                && player.Location.X > 0
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X - 1, player.Location.Y].Id == 16;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctfBot"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private static bool CanDigRight(CaptureTheFlagBot ctfBot, Player player)
        {
            return player.HorizontalDirection == HorizontalDirection.Right
                && player.Location.X < ctfBot.JoinedWorld.Width
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X + 1, player.Location.Y].Id == 16;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctfBot"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private static bool CanDigUp(CaptureTheFlagBot ctfBot, Player player)
        {
            return player.VerticalDirection == VerticalDirection.Up
                && player.Location.Y > 0
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y - 1].Id == 16;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctfBot"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private static bool CanDigDown(CaptureTheFlagBot ctfBot, Player player)
        {
            return player.VerticalDirection == VerticalDirection.Down
                && player.Location.Y < ctfBot.JoinedWorld.Height
                && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, player.Location.X, player.Location.Y + 1].Id == 16;
        }
    }
}