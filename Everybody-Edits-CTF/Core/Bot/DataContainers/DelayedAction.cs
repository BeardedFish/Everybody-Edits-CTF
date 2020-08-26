// File Name:     DelayedAction.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 20, 2020

using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Bot.DataContainers
{
    public abstract class DelayedAction
    {
        /// <summary>
        /// The delay a player has to wait in order to perform an action again.
        /// </summary>
        public int DelayMs { get; private set; }

        /// <summary>
        /// A dictionary which keeps track of players last tick time, in milliseconds.
        /// </summary>
        public Dictionary<Player, long> lastPlayerTickMs { get; private set; }

        /// <summary>
        /// Abstract class which keeps track of <see cref="Player"/> time ticks, in milliseconds.
        /// </summary>
        /// <param name="delayMs">The delay in milliseconds all players must wait in order to perform an action again.</param>
        public DelayedAction(CaptureTheFlagBot ctfBot, int delayMs)
        {
            DelayMs = delayMs;
            lastPlayerTickMs = new Dictionary<Player, long>();

            ctfBot.OnPlayerJoined += OnPlayerJoined;
            ctfBot.OnPlayerLeft += OnPlayerLeft;
        }

        /// <summary>
        /// Event handler for when a player joins the Everybody Edits world. This method adds the player to the <see cref="lastPlayerTickMs"/> dictionary via the
        /// <see cref="UpdatePlayerCurrentTick"/> method.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that joined the world.</param>
        private void OnPlayerJoined(CaptureTheFlagBot ctfBot, Player player)
        {
            UpdatePlayerCurrentTick(player);
        }

        /// <summary>
        /// Event handler for when a player leaves the Everybody Edits world. This method removes the player from the <see cref="lastPlayerTickMs"/> dictionary if they exist
        /// in it.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player that left the world.</param>
        private void OnPlayerLeft(CaptureTheFlagBot ctfBot, Player player)
        {
            if (lastPlayerTickMs.ContainsKey(player))
            {
                lastPlayerTickMs.Remove(player);
            }
        }

        /// <summary>
        /// Updates the <see cref="lastPlayerTickMs"/> dictionary by logging the current current millisecond time tick.
        /// </summary>
        /// <param name="player">The player to be updated.</param>
        protected void UpdatePlayerCurrentTick(Player player)
        {
            if (!lastPlayerTickMs.ContainsKey(player))
            {
                lastPlayerTickMs.Add(player, 0);
            }

            lastPlayerTickMs[player] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// States whether the delay is over for a player or not.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>True if the player has waited the <see cref="DelayMs"/> value, if not, false.</returns>
        protected bool IsDelayOver(Player player)
        {
            if (lastPlayerTickMs.ContainsKey(player))
            {
                return DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastPlayerTickMs[player] >= DelayMs;
            }

            return false;
        }
    }
}