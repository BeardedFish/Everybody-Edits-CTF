// File Name:     EffectBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class EffectBlock : Block
    {
        /// <summary>
        /// 
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="typeId"></param>
        public EffectBlock(int id, int typeId) : base(id)
        {
            TypeId = typeId;
        }
    }
}
