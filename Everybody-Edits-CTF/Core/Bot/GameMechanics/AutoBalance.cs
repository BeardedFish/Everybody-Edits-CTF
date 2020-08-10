// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 20, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class AutoBalance
    {
        /// <summary>
        /// Auto balances the teams for the Capture The Flag game if the team that the player joined has more players than the opposite team. If that condition is true,
        /// then the player is transferred to the other team.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(Player player)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }
            
            string resultMsg = $"You joined the {TeamHelper.EnumToString(player.Team)} team!";
            int joinedTeamTotalPlayers = TeamHelper.TotalPlayers(JoinedWorld.Players, player.Team) - 1;
            int oppositeTeamTotalPlayers = TeamHelper.TotalPlayers(JoinedWorld.Players, TeamHelper.GetOppositeTeam(player.Team));

            if (GameSettings.AutoBalanceTeams)
            {
                if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
                {
                    resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                    Point teleLocation = player.Team == Team.Blue ? GameSettings.RedSpawnLocation : GameSettings.BlueSpawnLocation;
                    CaptureTheFlagBot.TeleportPlayer(player, teleLocation.X, teleLocation.Y);
                }
            }

            CaptureTheFlagBot.SendPrivateMessage(player, resultMsg);
        }
    }
}