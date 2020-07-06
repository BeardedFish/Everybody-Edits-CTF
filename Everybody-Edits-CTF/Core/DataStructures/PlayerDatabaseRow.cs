// File Name:     PlayerDatabaseRow.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

namespace Everybody_Edits_CTF.Core.DataStructures
{
    public class PlayerDatabaseRow
    {
        public string Username;
        public int TotalWins;
        public int TotalLosses;
        public int Coins;
        public int TotalKills;
        public bool IsNewPlayer;

        public PlayerDatabaseRow(string username, int totalWins, int totalLosses, int totalKills, int coins, bool isNewPlayer)
        {
            Username = username;
            TotalWins = totalWins;
            TotalLosses = totalLosses;
            TotalKills = totalKills;
            Coins = coins;
            IsNewPlayer = isNewPlayer;
        }
    }
}