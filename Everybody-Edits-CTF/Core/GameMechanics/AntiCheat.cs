// File Name:     AntiCheat.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Helpers;
using System.Text;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class AntiCheat
    {
        public static void Handle(Player player)
        {
            if (player.HasEnemyFlag && player.IsInGodMode)
            {
                CaptureTheFlag.ReturnFlag(player, false);

                StringBuilder enemyTeam = new StringBuilder(TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team)));
                enemyTeam[0] = char.ToUpper(enemyTeam[0]);

                CaptureTheFlagBot.SendChatMessage($"ANTI-CHEAT! Player {player.Username} has used god mode while carrying the {enemyTeam.ToString().ToLower()} teams flag!");
                CaptureTheFlagBot.SendChatMessage($"{enemyTeam} teams flag has been returned to base.");
            }
        }
    }
}