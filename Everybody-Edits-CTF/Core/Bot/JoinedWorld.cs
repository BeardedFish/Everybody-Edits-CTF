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
        public static int Width;

        public static int Height;

        public static Block[,,] Blocks;

        /// <summary>
        /// Contains all the players currently in the Everybody Edits world. The integer is the Player id while the Player is the object that contains data about the
        /// player.
        /// </summary>
        public static Dictionary<int, Player> Players { get; private set; } = new Dictionary<int, Player>();
    }
}