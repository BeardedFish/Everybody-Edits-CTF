﻿// File Name:     PlayerDatabaseRow.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

namespace Everybody_Edits_CTF.Core.Database
{
    public class PlayerDatabaseRow
    {
        /// <summary>
        /// The username of the player.
        /// </summary>
        public string Username;

        /// <summary>
        /// The total number of times that this player won a Capture the Flag game.
        /// </summary>
        public int TotalWins;

        /// <summary>
        /// The total number of times that this player lost a Capture the Flag game.
        /// </summary>
        public int TotalLosses;

        /// <summary>
        /// The total number of kills this player has accumulated while playing Capture the Flag.
        /// </summary>
        public int TotalKills;

        /// <summary>
        /// The amount of coins this player currently possesses.
        /// </summary>
        public int Coins;

        /// <summary>
        /// States whether this player is a new player or not. A new player is someone who does not exist in the MySql database.
        /// </summary>
        public bool IsNewPlayer;

        /// <summary>
        /// Constructor for holding data of a single player which will be saved to a MySql database.
        /// </summary>
        /// <param name="username">Refer to <see cref="Username"/> for description.</param>
        /// <param name="totalWins">Refer to <see cref="TotalWins"/> for description.</param>
        /// <param name="totalLosses">Refer to <see cref="TotalLosses"/> for description.</param>
        /// <param name="totalKills">Refer to <see cref="TotalKills"/> for description.</param>
        /// <param name="coins">Refer to <see cref="Coins"/> for description.</param>
        /// <param name="isNewPlayer">Refer to <see cref="IsNewPlayer"/> for description.</param>
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