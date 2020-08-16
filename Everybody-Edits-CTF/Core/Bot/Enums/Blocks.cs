// File Name:     Blocks.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, August 16, 2020
//
// NOTE:          Although this is a class and not an enum data type, it acts like one.

using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;

namespace Everybody_Edits_CTF.Core.Bot.Enums
{
    public static class Blocks
    {
        public static readonly Block None = new Block();

        /// <summary>
        /// Static class which contains foreground blocks which can be placed by the Capture The Flag bot.
        /// </summary>
        public static class Foreground
        {
            public static readonly Block FactoryWood = new Block(47);

            public static readonly Block Lava = new Block(416);

            public static readonly Block Caution = new Block(1058);

            public static readonly MorphableBlock BlueFlag = new MorphableBlock(327, 1);

            public static readonly MorphableBlock RedFlag = new MorphableBlock(327, 4);
        }

        /// <summary>
        /// Static class which contains background blocks which can be placed by the Capture The Flag bot.
        /// </summary>
        public static class Background
        {
            public static readonly Block BrownBrick = new Block(507);

            public static readonly Block DarkOrangeLava = new Block(629);
        }
    }
}