﻿// File Name:     Block.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class Block
    {
        /// <summary>
        /// The id of the block.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Construcor which creates a block with the id of zero.
        /// </summary>
        public Block()
        {
            Id = 0;
        }

        /// <summary>
        /// Constructor which creates a block of a specified id.
        /// </summary>
        /// <param name="id">The id of the block.</param>
        public Block(int id)
        {
            Id = id;
        }
    }
}