// File Name:     PortalBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks
{
    public class PortalBlock : Block
    {
        /// <summary>
        /// The id of the portal.
        /// </summary>
        public int PortalId { get; set; }
        
        /// <summary>
        /// The id of the portal which this portal leads too.
        /// </summary>
        public int PortalTargetId { get; set; }

        /// <summary>
        /// The rotation id of the portal block.
        /// 
        /// 0 - Left
        /// 1 - Up
        /// 2 = Right
        /// 3 = Down
        /// </summary>
        public int RotationId { get; set; }

        /// <summary>
        /// Constructor which creates a PortalBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="portalId">Refer to <see cref="PortalId"/> for description.</param>
        /// <param name="portalTarget">Refer to <see cref="PortalTargetId"/> for description.</param>
        /// <param name="rotationId">Refer to <see cref="RotationId"/> for description.</param>
        public PortalBlock(int id, int portalId, int portalTarget, int rotationId) : base(id)
        {
            PortalId = portalId;
            PortalTargetId = portalTarget;
            RotationId = rotationId;
        }
    }
}