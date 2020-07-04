// File Name:     TeamHelper.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Enums;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Helpers
{
    public static class TeamHelper
    {
        public static Team IdToEnum(int teamId)
        {
            switch (teamId)
            {
                case 1:
                    return Team.Red;
                case 2:
                    return Team.Blue;
                default:
                    return Team.None;
            }
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

        public static bool IsEnemyPlayer(Team p1, Team p2)
        {
            if (p1 == Team.None || p2 == Team.None)
            {
                return false;
            }

            return p1 != p2;
        }

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