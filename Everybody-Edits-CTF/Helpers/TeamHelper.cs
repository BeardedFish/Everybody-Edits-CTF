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
        /// <summary>
        /// Converts an integer to a Team enum.
        /// </summary>
        /// <param name="teamId">The integer to be converted to a Team enum.</param>
        /// <returns>
        /// If the team id is 1, then Team.Red is returned. If the team id is 2, then Team.Blue is returned. If the team id is neither of those, then Team.None is returned.
        /// </returns>
        public static Team IdToEnum(int teamId) => teamId switch
        {
            1 => Team.Red,
            2 => Team.Blue,
            _ => Team.None,
        };

        /// <summary>
        /// Converts a team enum to a human readable string.
        /// </summary>
        /// <param name="team">The Team enum to be converted to a human readable string.</param>
        /// <returns>A lowercased string that has the same name as the enum.</returns>
        public static string EnumToString(Team team) => team switch
        {
            Team.Blue => "blue",
            Team.Red => "red",
            _ => "none",
        };

        /// <summary>
        /// Gets the opposite team from a Team enum.
        /// </summary>
        /// <param name="team">The team that you want to get the opposite of.</param>
        /// <returns>
        /// If the team is blue, then Team.Red is returned. If the team is red, then Team.Blue is returned. If the team is neither red or blue, then Team.None is returned.
        /// </returns>
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