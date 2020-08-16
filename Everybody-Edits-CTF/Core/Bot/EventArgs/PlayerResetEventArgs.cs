// File Name:     PlayerResetEventArgs.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Bot.EventArgs
{
    public class PlayerResetEventArgs
    {
        /// <summary>
        /// States whether the properties of the player (or players) was/were reset or not.
        /// </summary>
        public bool PropertiesReset { get; set; }

        /// <summary>
        /// A list of players who were reset.
        /// </summary>
        public List<Player> PlayersReset { get; set; }

        /// <summary>
        /// Constructor for a <see cref="PlayerResetEventArgs"/> object which stores data about when a player (or players) is/are reset in Everybody Edits.
        /// </summary>
        /// <param name="propertiesReset">Refer to <see cref="PropertiesReset"/> for description.</param>
        /// <param name="playersReset">Refer to <see cref="PlayersReset"/> for description.</param>
        public PlayerResetEventArgs(bool propertiesReset, List<Player> playersReset)
        {
            PropertiesReset = propertiesReset;
            PlayersReset = playersReset;
        }
    }
}