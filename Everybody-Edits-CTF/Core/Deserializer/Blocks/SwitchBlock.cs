// File Name:     SwitchBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class SwitchBlock : Block
    {
        /// <summary>
        /// 
        /// </summary>
        public uint GateTriggerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="gateTriggerId"></param>
        public SwitchBlock(int id, uint gateTriggerId) : base(id)
        {
            GateTriggerId = gateTriggerId;
        }
    }
}