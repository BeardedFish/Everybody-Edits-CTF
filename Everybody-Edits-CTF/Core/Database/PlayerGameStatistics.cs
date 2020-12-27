// File Name:     PlayerGameStatistics.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, August 19, 2020

using System;

namespace Everybody_Edits_CTF.Core.Database
{
    public sealed class PlayerGameStatistics : ICloneable
    {
        /// <summary>
        /// The total number of Capture The Flag wins the player achieved.
        /// </summary>
        public int TotalWins { get; set; }

        /// <summary>
        /// The total number of Capture The Flag losses the player achieved.
        /// </summary>
        public int TotalLosses { get; set; }

        /// <summary>
        /// The total number of kills the player achieved while playing Capture The Flag.
        /// </summary>
        public int TotalKills { get; set; }

        /// <summary>
        /// The total number of coins the player has.
        /// </summary>
        public int Coins { get; set; }

        /// <summary>
        /// Creates a <see cref="PlayerGameStatistics"/> object with default values. The default value is zero.
        /// </summary>
        public PlayerGameStatistics() : this(0, 0, 0, 0)
        {
        
        }

        /// <summary>
        /// Creates a <see cref="PlayerGameStatistics"/> object with specfied values.
        /// </summary>
        /// <param name="totalWins">Refer to <see cref="TotalWins"/> for description.</param>
        /// <param name="totalLosses">Refer to <see cref="TotalLosses"/> for description.</param>
        /// <param name="totalKills">Refer to <see cref="TotalKills"/> for description.</param>
        /// <param name="coins">Refer to <see cref="Coins"/> for description.</param>
        public PlayerGameStatistics(int totalWins, int totalLosses, int totalKills, int coins)
        {
            TotalWins = totalWins;
            TotalLosses = totalLosses;
            TotalKills = totalKills;
            Coins = coins;
        }

#nullable enable
        /// <summary>
        /// States whether an object is equal to this <see cref="PlayerGameStatistics"/> instance or not.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>True if the object is equal to this instance, if not, false.</returns>
        public override bool Equals(object? obj)
        {
            PlayerGameStatistics? pgs = obj as PlayerGameStatistics;

            return obj != null && TotalWins == pgs?.TotalWins && TotalKills == pgs?.TotalKills && pgs?.TotalLosses == pgs?.TotalLosses && Coins == pgs?.Coins;
        }
#nullable disable

        /// <summary>
        /// Gets the unique hash code of this object instance.
        /// </summary>
        /// <returns>An integer that states the hash code of this object instance.</returns>
        public override int GetHashCode()
        {
            int hashCode = -628107557;

            hashCode = hashCode * -1521134295 + TotalWins.GetHashCode();
            hashCode = hashCode * -1521134295 + TotalLosses.GetHashCode();
            hashCode = hashCode * -1521134295 + TotalKills.GetHashCode();
            hashCode = hashCode * -1521134295 + Coins.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// Clones this object by creating a new <see cref="PlayerGameStatistics"/> object with the same exact values as this instance.. This object must be casted to
        /// a <see cref="PlayerGameStatistics"/> in order for the values to be accesses.
        /// </summary>
        /// <returns>An object that is an exact clone of this instance.</returns>
        public object Clone()
        {
            return new PlayerGameStatistics(TotalWins, TotalLosses, TotalKills, Coins);
        }
    }
}