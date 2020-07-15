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
        /// Constructor which creates a TimedEffectBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="duration">Refer to <see cref="Duration"/> for description.</param>
        public TimedEffectBlock(int id, int duration) : base(id)
        {
            Duration = duration;
        }
    }
}