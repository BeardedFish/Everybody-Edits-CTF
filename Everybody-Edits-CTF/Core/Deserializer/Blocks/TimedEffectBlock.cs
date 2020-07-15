// File Name:     TimedEffectBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class TimedEffectBlock : Block
    {
        /// <summary>
        /// The duration for how long the effect will last.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="duration"></param>
        public TimedEffectBlock(int id, int duration) : base(id)
        {
            Duration = duration;
        }
    }
}