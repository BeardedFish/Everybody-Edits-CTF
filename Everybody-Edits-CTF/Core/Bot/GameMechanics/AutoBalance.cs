// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class AutoBalance
    {
        /// <summary>
        /// Handles team balance in the Capture The Flag game. If a player joins a team with more players than the other eam, then they are transferred to the other team.
        /// /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }

            string resultMsg = $"You joined the {TeamHelper.EnumToString(player.Team)} team!";
            int joinedTeamTotalPlayers = TeamHelper.TotalPlayers(ctfBot.JoinedWorld.Players, player.Team) - 1;
            int oppositeTeamTotalPlayers = TeamHelper.TotalPlayers(ctfBot.JoinedWorld.Players, TeamHelper.GetOppositeTeam(player.Team));

            if (GameSettings.AutoBalanceTeams)
            {
                if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
                {
                    resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                    Point teleLocation = player.Team == Team.Blue ? GameSettings.RedCheckpointLocation : GameSettings.BlueCheckpointLocation;
                    ctfBot.TeleportPlayer(player, teleLocation.X, teleLocation.Y);
                }
            }

            ctfBot.SendPrivateMessage(player, resultMsg);
        }
    }
}