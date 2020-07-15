// File Name:     GameSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 6, 2020

using System.Drawing;

namespace Everybody_Edits_CTF.Core.Settings
{
    public static class GameSettings
    {
        /// <summary>
        /// States whether the bot will auto balance teams if they are unbalanced.
        /// </summary>
        public const bool AutoBalanceTeams = true;

        /// <summary>
        /// States the max score a team needs to achieve in order to win the game.
        /// </summary>
        public const ushort MaxScoreToWin = 5;

        /// <summary>
        /// The locations of the blue/red team's flag in the world that the bot joined.
        /// </summary>
        public static readonly Point BlueFlagLocation = new Point(32, 144), RedFlagLocation = new Point(367, 144);

        /// <summary>
        /// The amount of time, in milliseconds, a player has to wait in the <see cref="RespawnCooldownZone"/> before respawning back into the game.
        /// </summary>
        public const ushort RespawnCooldownMs = 5000;

        /// <summary>
        /// The location where a player of a specified team will be teleported when they die to wait the <see cref=">RespawnCooldownMs"/>..
        /// </summary>
        public static readonly Point BlueRespawnCooldownLocation = new Point(1, 1), RedRespawnCooldownLocation = new Point(398, 1);

        /// <summary>
        /// The location where a player of a specified team is teleported after they are finished waiting the <see cref="RespawnCooldownMs"/>.
        /// </summary>
        public static readonly Point BlueSpawnLocation = new Point(1, 6), RedSpawnLocation = new Point(398, 6);
    }
}