// File Name:     AntiCheat.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class AntiCheat : IGameMechanic
    {
        /// <summary>
        /// Handles any player cheating during the Capture The Flag game. The way the player is handled depends on the <see cref="EverybodyEditsMessage"/> type.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(CtfBot ctfBot, string messageType, Player player)
        {
            if (messageType == EverybodyEditsMessage.GodModeToggled)
            {
                // Remove flag from player if they have the enemy flag and enter God mode
                if (player.IsPlayingGame
                    && player.IsInGodMode
                    && player.HasEnemyFlag(ctfBot))
                {
                    ctfBot.SendChatMessage($"ANTI-CHEAT! Player {player.Username} has used God mode while carrying the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag!");

                    ctfBot.CurrentGameRound.FlagSystem.Flags[TeamHelper.GetOppositeTeam(player.Team)].Return(ctfBot, null, false);
                }
            }
            else if (messageType == EverybodyEditsMessage.Effect)
            {
                if (!player.PurchasedItemLegally)
                {
                    ctfBot.KickPlayer(player.Username, "You were kicked by the anti-cheat system for attemping to cheat.");
                }
            }
        }
    }
}