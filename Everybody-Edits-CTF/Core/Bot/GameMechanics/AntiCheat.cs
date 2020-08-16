// File Name:     AntiCheat.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventArgs;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class AntiCheat
    {
        /// <summary>
        /// Handles a player cheating if they enter God mode. A player is considered cheating when they are holding the enemy flag their <see cref="Player.IsInGodMode"/> status
        /// is set to true.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void HandleGodModeCheater(CaptureTheFlagBot ctfBot, Player player)
        {
            // Remove flag from player if they have the enemy flag and enter God mode
            if (player.IsPlayingGame && player.IsInGodMode && player.HasEnemyFlag(ctfBot))
            {
                ctfBot.SayChatMessage($"ANTI-CHEAT! Player {player.Username} has used God mode while carrying the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag!");

                ctfBot.FlagSystem.Flags[TeamHelper.GetOppositeTeam(player.Team)].Return(ctfBot, null, false);
            }
        }

        /// <summary>
        /// Handles a player cheating if they take an effect they did not legally purchase in the <see cref="Shop"/>.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="eventArgs">The arguments for when the player received/lost an effect.</param>
        public static void HandleEffectCheater(CaptureTheFlagBot ctfBot, EffectToggledEventArgs eventArgs)
        {
            if (!ctfBot.FinishedInit || eventArgs.Effect == Effect.Fire || !eventArgs.IsEffectEnabled)
            {
                return;
            }

            if (eventArgs.Effect != eventArgs.Player.PurchasedEffectFlag)
            {
                ctfBot.KickPlayer(eventArgs.Player.Username, "You were auto kicked for attemping to cheat!");
            }

            // Reset the flag variable
            eventArgs.Player.PurchasedEffectFlag = Effect.None;
        }
    }
}