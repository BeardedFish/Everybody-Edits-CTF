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
        public Dictionary<Player, long> LastPlayerTickMs { get; private set; }

        /// <summary>
        /// Abstract class which keeps track of <see cref="Player"/> time ticks, in milliseconds.
        /// </summary>
        /// <param name="delayMs">The delay in milliseconds all players must wait in order to perform an action again.</param>
        public DelayedAction(int delayMs)
        {
            DelayMs = delayMs;
            LastPlayerTickMs = new Dictionary<Player, long>();
        }

        /// <summary>
        /// Updates the <see cref="LastPlayerTickMs"/> dictionary by logging the current current u
        /// </summary>
        /// <param name="player">The player to be updated.</param>
        protected void UpdatePlayerCurrentTick(Player player)
        {
            if (!LastPlayerTickMs.ContainsKey(player))
            {
                LastPlayerTickMs.Add(player, 0);
            }

            LastPlayerTickMs[player] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// States whether the delay is over for a player or not.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>True if the player has waited the <see cref="DelayMs"/> value, if not, false.</returns>
        protected bool IsDelayOver(Player player)
        {
            if (LastPlayerTickMs.ContainsKey(player))
            {
                return DateTimeOffset.Now.ToUnixTimeMilliseconds() - LastPlayerTickMs[player] >= DelayMs;
            }

            return false;
        }
    }
}