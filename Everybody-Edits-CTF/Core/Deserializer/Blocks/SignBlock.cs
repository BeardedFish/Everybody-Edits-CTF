// File Name:     SignBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class SignBlock : Block
    {
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ColourId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="text"></param>
        /// <param name="rotationId"></param>
        public SignBlock(int id, string text, int rotationId) : base(id)
        {
            Text = text;
            ColourId = rotationId;
        }
    }
}