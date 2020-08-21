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
        public int DelayMs { get; private set; };

        /// <summary>
        /// A dictionary which keeps track of a players last tick time, in milliseconds.
        /// </summary>
        public Dictionary<Player, long> LastPlayerTickMs { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delayMs"></param>
        public DelayedAction(int delayMs)
        {
            DelayMs = delayMs;
            LastPlayerTickMs = new Dictionary<Player, long>();
        }

        /// <summary>
        /// Updates the <see cref="LastPlayerTickMs"/> dictionary by logging the current current u
        /// </summary>
        /// <param name="player"></param>
        protected void UpdatePlayerCurrentTick(Player player)
        {
            if (!LastPlayerTickMs.ContainsKey(player))
            {
                LastPlayerTickMs.Add(player, 0);
            }

            LastPlayerTickMs[player] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// States whether a player has waited the <see cref="DelayMs"/> since their last time tick logged in the <see cref="LastPlayerTickMs"/> dictionary.
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