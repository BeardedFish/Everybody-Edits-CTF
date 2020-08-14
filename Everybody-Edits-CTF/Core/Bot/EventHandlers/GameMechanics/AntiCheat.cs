// File Name:     AntiCheat.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class AntiCheat : IGameMechanic
    {
        public void Handle(string messageType, Player player)
        {
            if (messageType == EverybodyEditsMessage.GodModeToggled)
            {
                // Remove flag from player if they have the enemy flag and enter God mode
                if (player.IsPlayingGame
                    && player.IsInGodMode
                    && player.HasEnemyFlag)
                {
                    CtfBot.SendChatMessage($"ANTI-CHEAT! Player {player.Username} has used God mode while carrying the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag!");

                    JoinedWorld.FlagSystem.Flags[TeamHelper.GetOppositeTeam(player.Team)].Return(null, false);
                }
            }
        }
    }
}