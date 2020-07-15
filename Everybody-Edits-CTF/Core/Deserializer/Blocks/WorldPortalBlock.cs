// File Name:     WorldPortalBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

namespace Everybody_Edits_CTF.Core.Deserializer.Blocks
{
    public class WorldPortalBlock : Block
    {
        /// <summary>
        /// The id of the world that this portal leads to.
        /// </summary>
        public string TargetWorldId { get; set; }

        /// <summary>
        /// The spawn position that the player spawns to when joining the <see cref="TargetWorldId"/>.
        /// </summary>
        public int TargetSpawn { get; set; }

        /// <summary>
        /// Constructor which creates a WorldPortalBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="targetWorldId">Refer to <see cref="TargetWorldId"/> for description.</param>
        /// <param name="targetSpawn">Refer to <see cref="TargetSpawn"/> for description.</param>
        public WorldPortalBlock(int id, string targetWorldId, int targetSpawn) : base(id)
        {
            TargetWorldId = targetWorldId;
            TargetSpawn = targetSpawn;
        }
    }
}