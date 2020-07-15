// File Name:     NonPlayableCharacterBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class NonPlayableCharacterBlock : Block
    {
        /// <summary>
        /// The name of the Non Playable Character.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The messages that the Non Playable Character can say. This will always have a length of three, however, values can be null.
        /// </summary>
        public string[] Messages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="name"></param>
        /// <param name="messages"></param>
        public NonPlayableCharacterBlock(int id, string name, string[] messages) : base(id)
        {
            Name = name;
            Messages = messages;
        }
    }
}