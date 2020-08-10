// File Name:     MusicBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

namespace Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks
{
    public class MusicBlock : Block
    {
        /// <summary>
        /// The sound id of this music block.
        /// </summary>
        public int SoundId { get; set; }

        /// <summary>
        /// Constructor which creates a MusicBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="soundId">Refer to <see cref="SoundId"/> for description.</param>
        public MusicBlock(int id, int soundId) : base(id)
        {
            SoundId = soundId;
        }
    }
}