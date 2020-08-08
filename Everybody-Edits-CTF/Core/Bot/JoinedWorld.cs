// File Name:     JoinedWorldInformation.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.Deserializer.Blocks;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Bot
{
    public static class JoinedWorld
    {
        /// <summary>
        /// The width of the Everybody Edits world that the bot joined.
        /// </summary>
        public static int Width;

        /// <summary>
        /// The height of the Everybody Edits world that the bot joined.
        /// </summary>
        public static int Height;

        /// <summary>
        /// The array of blocks in the Everybody Edits world. This array will have a length of <see cref="Width"/> and a width of <see cref="Height"/>.
        /// </summary>
        public static Block[,,] Blocks;

        /// <summary>
        /// Contains all the players currently in the Everybody Edits world. The integer is the Player id while the Player is the object that contains data about the
        /// player.
        /// </summary>
        public static Dictionary<int, Player> Players { get; private set; } = new Dictionary<int, Player>();
    }
}