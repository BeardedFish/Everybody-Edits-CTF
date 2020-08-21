// File Name:     GameSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 6, 2020

using System.Drawing;

namespace Everybody_Edits_CTF.Core.Settings
{
    public static class GameSettings
    {
        /// <summary>
        /// The locations of the blue/red team's flag in the world that the bot joined.
        /// </summary>
        public static readonly Point BlueFlagLocation = new Point(32, 101), RedFlagLocation = new Point(367, 101);

        /// <summary>
        /// The location where a player of a specified team will be teleported when they die to wait the <see cref=">RespawnCooldownMs"/>.
        /// </summary>
        public static readonly Point BlueRespawnCooldownLocation = new Point(159, 1), RedRespawnCooldownLocation = new Point(239, 1);

        /// <summary>
        /// Where a player will respawn after waiting the <see cref="RespawnCooldownMs"/>.
        /// </summary>
        public static readonly Point BlueCheckpointLocation = new Point(BlueRespawnCooldownLocation.X, BlueRespawnCooldownLocation.Y + 5), RedCheckpointLocation = new Point(RedRespawnCooldownLocation.X, RedRespawnCooldownLocation.Y + 5);

        /// <summary>
        /// States the max score a team needs to achieve in order to win the game.
        /// </summary>
        public const ushort MaxScoreToWin = 5;

        /// <summary>
        /// The amount of time, in milliseconds, a player has to wait in the <see cref="RespawnCooldownZone"/> before respawning back into the game.
        /// </summary>
        public const ushort RespawnCooldownMs = 5000;
    }
}