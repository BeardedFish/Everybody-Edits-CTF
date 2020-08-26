// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;
using Everybody_Edits_CTF.Core.Settings;
using System.Collections.Generic;
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

            string resultMsg = $"You joined the {player.Team.GetStringName()} team!";
            int joinedTeamTotalPlayers = CountPlayers(ctfBot.JoinedWorld.Players, player.Team) - 1;
            int oppositeTeamTotalPlayers = CountPlayers(ctfBot.JoinedWorld.Players, player.Team.GetOppositeTeam());

            if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
            {
                resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                Point teleLocation = player.Team == Team.Blue ? GameSettings.RedCheckpointLocation : GameSettings.BlueCheckpointLocation;
                ctfBot.TeleportPlayer(player, teleLocation.X, teleLocation.Y);
            }

            ctfBot.SendPrivateMessage(player, resultMsg);
        }

        /// <summary>
        /// Gets the total number of players on a specific team.
        /// </summary>
        /// <param name="playersInWorld">The dictionary of players in the Everybody Edits world.</param>
        /// <param name="team">The team a player must belong to in order to be accumulated.</param>
        /// <returns></returns>
        private static int CountPlayers(Dictionary<int, Player> playersInWorld, Team team)
        {
            int total = 0;

            foreach (Player player in playersInWorld.Values)
            {
                if (player.Team == team)
                {
                    total++;
                }
            }

            return total;
        }
    }
}