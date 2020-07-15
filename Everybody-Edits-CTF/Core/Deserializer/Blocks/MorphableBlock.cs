// File Name:     MorphableBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class MorphableBlock : Block
    {
        /// <summary>
        /// The morph id of the block.
        /// </summary>
        public int MorphId { get; set; }

        /// <summary>
        /// Constructor which creates a MorphableBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="morphId">Refer to <see cref="MorphId"/> for description.</param>
        public MorphableBlock(int id, int morphId) : base(id)
        {
            MorphId = morphId;
        }
    }
}