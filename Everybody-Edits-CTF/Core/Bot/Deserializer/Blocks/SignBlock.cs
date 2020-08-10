// File Name:     SignBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks
{
    public class SignBlock : Block
    {
        /// <summary>
        /// The text on the sign.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The colour id of the sign.
        /// </summary>
        public int ColourId { get; set; }

        /// <summary>
        /// Constructor which creates a SignBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="text">Refer to <see cref="Text"/> for description.</param>
        /// <param name="colourId">Refer to <see cref="ColourId"/> for description.</param>
        public SignBlock(int id, string text, int colourId) : base(id)
        {
            Text = text;
            ColourId = colourId;
        }
    }
}