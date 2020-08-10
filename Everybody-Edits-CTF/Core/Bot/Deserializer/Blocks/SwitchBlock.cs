// File Name:     SwitchBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks
{
    public class SwitchBlock : Block
    {
        /// <summary>
        /// 
        /// </summary>
        public uint GateTriggerId { get; set; }

        /// <summary>
        /// Constructor which creates a SwitchBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="gateTriggerId">Refer to <see cref="GateTriggerId"/> for description.</param>
        public SwitchBlock(int id, uint gateTriggerId) : base(id)
        {
            GateTriggerId = gateTriggerId;
        }
    }
}