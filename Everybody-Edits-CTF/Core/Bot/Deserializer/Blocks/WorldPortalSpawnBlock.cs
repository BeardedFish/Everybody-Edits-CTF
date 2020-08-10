// File Name:     WorldPortalSpawnBlock.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, July 16, 2020

namespace Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks
{
    public class WorldPortalSpawnBlock : Block
    {
        /// <summary>
        /// The id of this spawn block.
        /// </summary>
        public int SpawnId { get; set; }

        /// <summary>
        /// Constructor which creates a WorldPortalSpawnBlock object.
        /// </summary>
        /// <param name="id">Refer to <see cref="Block.Id"/> for description.</param>
        /// <param name="spawnId">Refer to <see cref="SpawnId"/> for description.</param>
        public WorldPortalSpawnBlock(int id, int spawnId) : base(id)
        {
            SpawnId = spawnId;
        }
    }
}