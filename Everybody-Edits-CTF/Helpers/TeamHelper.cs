// File Name:     TeamHelper.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Helpers
{
    public static class TeamHelper
    {
        public static Team IdToEnum(int teamId)
        {
            return teamId switch
            {
                1 => Team.Red,
                2 => Team.Blue,
                _ => Team.None,
            };
        }

        public static string EnumToString(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return "blue";
                case Team.Red:
                    return "red";
                default:
                    return "none";
            }
        }

        public static Team GetOppositeTeam(Team team)
        {
            if (team == Team.Blue)
            {
                return Team.Red;
            }

            if (team == Team.Red)
            {
                return Team.Blue;
            }

            return Team.None;
        }

        /// <summary>
        /// Gets the total number of players on a specific team.
        /// </summary>
        /// <param name="playersInWorld">The dictionary of players in the Everybody Edits world.</param>
        /// <param name="team">The team a player must belong to in order to be accumulated.</param>
        /// <returns></returns>

        public static int TotalPlayers(Dictionary<int, Player> playersInWorld, Team team)
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