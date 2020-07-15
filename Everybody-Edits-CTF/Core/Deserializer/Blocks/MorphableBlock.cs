// File Name:     MorphableBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class MorphableBlock : Block
    {
        /// <summary>
        /// 
        /// </summary>
        int MorphId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="rotationId"></param>
        public MorphableBlock(int id, int rotationId) : base(id)
        {
            MorphId = MorphId;
        }
    }
}