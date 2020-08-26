// File Name:     RespawnSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;
using Everybody_Edits_CTF.Core.Bot.EventArgs;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class RespawnSystem
    {
        /// <summary>
        /// The amount of time, in milliseconds, a player has to wait in the <see cref="RespawnCooldownZone"/> before respawning back into the game.
        /// </summary>
        public const ushort RespawnCooldownMs = 5000;

        /// <summary>
        /// The location where a player of a specified team will be teleported when they die to wait the <see cref=">RespawnCooldownMs"/>.
        /// </summary>
        public static readonly Point BlueRespawnCooldownLocation = new Point(159, 1), RedRespawnCooldownLocation = new Point(239, 1);

        /// <summary>
        /// The location where a player will respawn after waiting the <see cref="RespawnCooldownMs"/>.
        /// </summary>
        public static readonly Point BlueCheckpointLocation = new Point(BlueRespawnCooldownLocation.X, BlueRespawnCooldownLocation.Y + 5), RedCheckpointLocation = new Point(RedRespawnCooldownLocation.X, RedRespawnCooldownLocation.Y + 5);

        /// <summary>
        /// Handles the respawn system for the Capture The Flag game. The respawn system teleports a player to their respective respawn cooldown location, which depends on
        /// which team they are currently on. If a player is holding the flag and this method gets called, then the flag that they're holding is returned to its base.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="eventArgs">The arguments for when the player (or players) was/were reset.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, PlayerResetEventArgs eventArgs)
        {
            if (!eventArgs.PropertiesReset)
            {
                foreach (Player player in eventArgs.PlayersReset)
                {
                    if (player.HasEnemyFlag(ctfBot))
                    {
                        Team enemyTeam = player.Team.GetOppositeTeam();

                        if (ctfBot.FlagSystem.Flags[enemyTeam].Holder == player)
                        {
                            ctfBot.SayChatMessage($"Player {player.Username} died while holding {enemyTeam.GetStringName()} teams flag.");

                            ctfBot.FlagSystem.Flags[enemyTeam].Return(ctfBot, null, false);
                        }

                        ctfBot.RemoveEffects(player);
                    }

                    RespawnPlayer(ctfBot, player);
                }
            }
        }

        /// <summary>
        /// Extension method which states whether a <see cref="Player"/> is respawning or not.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>True if the player is respawning, if not, false.</returns>
        public static bool IsRespawning(this Player player)
        {
            return player.Location == BlueRespawnCooldownLocation || player.Location == RedRespawnCooldownLocation;
        }
        
        /// <summary>
        /// Respawns a player in the Capture The Flag game. If this method is called on a player that is either not playing or is respawning then this method does nothing. The
        /// location where the player respawns depends on their team.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to respawn.</param>
        private static void RespawnPlayer(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame || player.IsRespawning())
            {
                return;
            }

            player.RestoreHealth();

            Task.Run(async() =>
            {
                Point respawnCooldownLocation = player.Team == Team.Blue ? BlueRespawnCooldownLocation : RedRespawnCooldownLocation;
                ctfBot.TeleportPlayer(player, respawnCooldownLocation.X, respawnCooldownLocation.Y);

                await Task.Delay(RespawnCooldownMs);

                Point respawnLocation = player.Team == Team.Blue ? BlueCheckpointLocation : RedCheckpointLocation;
                ctfBot.TeleportPlayer(player, respawnLocation.X, respawnLocation.Y);
            });
        }
    }
}