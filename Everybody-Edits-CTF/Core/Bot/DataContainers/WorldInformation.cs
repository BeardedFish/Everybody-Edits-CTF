// File Name:     WorldInformation.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.DataContainers
{
    public class WorldInformation
    {
        /// <summary>
        /// The array of blocks in the Everybody Edits world. This array will have a length of <see cref="Width"/> and a width of <see cref="Height"/>.
        /// </summary>
        public Block[,,] Blocks { get; set; }

        /// <summary>
        /// The millisecond time a curse effect was last removed from a player in the Everybody Edits world.
        /// </summary>
        public long LastCurseRemoveTickMs { get; set; }

        /// <summary>
        /// Contains all the players currently in the Everybody Edits world. The integer is the Player id while the Player is the object that contains data about the
        /// player.
        /// </summary>
        public Dictionary<int, Player> Players { get; private set; } = new Dictionary<int, Player>();

        /// <summary>
        /// The height of the Everybody Edits world that the bot joined, in blocks.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The width of the Everybody Edits world that the bot joined, in blocks.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets the total number of zombie players in the Everybody Edits world. A player who is a zombie has the <see cref="Player.IsZombie"/> variable set to true.
        /// </summary>
        public int TotalZombiePlayers
        {
            get
            {
                return Players.Count(player => player.Value.IsZombie);
            }
        }

        /// <summary>
        /// Searches the <see cref="Players"/> dictionary for a <see cref="Player"/> of a specified username and returns the id of the player.
        /// </summary>
        /// <param name="username">The username of the player to be searched for.</param>
        /// <returns>If the user is found, then their player id is returned. If the user is not found, then -1 is returned.</returns>
        public int GetPlayerId(string username)
        {
            foreach (KeyValuePair<int, Player> pair in Players)
            {
                if (pair.Value.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Key;
                }
            }

            return -1;
        }
    }
}