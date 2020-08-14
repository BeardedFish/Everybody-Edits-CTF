// File Name:     JoinedWorld.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.EventHandlers;
using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Bot.DataStructures
{
    public static class JoinedWorld
    {
        /// <summary>
        /// The anti-cheat system for the world that the bot joined.
        /// </summary>
        public static readonly AntiCheat AntiCheat = new AntiCheat();

        /// <summary>
        /// 
        /// </summary>
        public static readonly FlagSystem FlagSystem = new FlagSystem();

        /// <summary>
        /// The width of the Everybody Edits world that the bot joined.
        /// </summary>
        public static int Width { get; set; }

        /// <summary>
        /// The height of the Everybody Edits world that the bot joined.
        /// </summary>
        public static int Height { get; set; }

        /// <summary>
        /// The array of blocks in the Everybody Edits world. This array will have a length of <see cref="Width"/> and a width of <see cref="Height"/>.
        /// </summary>
        public static Block[,,] Blocks { get; set; }

        /// <summary>
        /// Contains all the players currently in the Everybody Edits world. The integer is the Player id while the Player is the object that contains data about the
        /// player.
        /// </summary>
        public static Dictionary<int, Player> Players { get; private set; } = new Dictionary<int, Player>();

        /// <summary>
        /// Searches the <see cref="Players"/> dictionary for a <see cref="Player"/> of a specified username and returns the id of the player.
        /// </summary>
        /// <param name="username">The username of the player to be searched for.</param>
        /// <returns>If the user is found, then their player id is returned. If the user is not found, then -1 is returned.</returns>
        public static int GetPlayerId(string username)
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