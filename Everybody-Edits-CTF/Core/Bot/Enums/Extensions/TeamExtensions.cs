// File Name:     TeamExtensions.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, August 25, 2020

namespace Everybody_Edits_CTF.Core.Bot.Enums.Extensions
{
    public static class TeamExtensions
    {
        /// <summary>
        /// Extension method for the <see cref="Team"/> enum which gets the opposite team of the enum.
        /// </summary>
        /// <param name="team">The team to get the opposite of.</param>
        /// <returns>
        /// If the team is blue, then <see cref="Team.Red"/> is returned. If the team is red, then <see cref="Team.Blue"/> is returned. If the team is neither red or blue,
        /// then <see cref="Team.None"/> is returned.
        /// </returns>
        public static Team GetOppositeTeam(this Team team)
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
        /// Extension method for the <see cref="Team"/> enum which converts the enum to a human readable string.
        /// </summary>
        /// <param name="team">The team enum to be converted to a human readable string.</param>
        /// <returns>A lowercased string that has the same name as the enum.</returns>
        public static string GetStringName(this Team team) => team switch
        {
            Team.Blue => "blue",
            Team.Red => "red",
            _ => "none",
        };
    }
}