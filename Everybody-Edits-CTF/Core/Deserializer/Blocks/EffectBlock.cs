// File Name:     EffectBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class EffectBlock : Block
    {
        /// <summary>
        /// The type of effect.
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Constructor which creates a EffectBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="typeId">Refer to <see cref="TypeId"/> for description.</param>
        public EffectBlock(int id, int typeId) : base(id)
        {
            TypeId = typeId;
        }
    }
}
