// File Name:     RespawnSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;
using Everybody_Edits_CTF.Core.Bot.EventArgs;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class RespawnSystem
    {
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

                    player.Respawn(ctfBot);
                }
            }
        }
    }
}