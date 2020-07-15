// File Name:     PortalBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class PortalBlock : Block
    {
        /// <summary>
        /// 
        /// </summary>
        public int PortalId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int PortalTarget { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RotationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="portalId"></param>
        /// <param name="portalTarget"></param>
        /// <param name="rotationId"></param>
        public PortalBlock(int id, int portalId, int portalTarget, int rotationId) : base(id)
        {
            PortalId = portalId;
            PortalTarget = portalTarget;
            RotationId = rotationId;
        }
    }
}