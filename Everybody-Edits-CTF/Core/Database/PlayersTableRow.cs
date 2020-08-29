// File Name:     PlayersTableRow.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

using System;

namespace Everybody_Edits_CTF.Core.Database
{
    public class PlayersTableRow
    {
        /// <summary>
        /// The username of the player.
        /// </summary>
        public readonly string Username;

        /// <summary>
        /// The date the player last joined the world.
        /// </summary>
        public DateTime LastVisitDate { get; set; }

        /// <summary>
        /// States whether the player is an administrator or not.
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// States whether the player is banned from the Everybody Edits world or not.
        /// </summary>
        public bool IsBanned { get; set; }

        /// <summary>
        /// States whether this player is a new player or not. A new player is someone who does not exist in the MySql database.
        /// </summary>
        public bool IsNewPlayer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PlayerGameStatistics Statistics { get; private set; }

        /// <summary>
        /// States whether the data of this player was modified or not.
        /// </summary>
        public bool ChangesOccured
        {
            get
            {
                return LastVisitDate != initialLastVisitDate
                    || IsAdministrator != initialIsAdministrator
                    || IsBanned != initialIsBanned
                    || !Statistics.Equals(initalStatistics)
                    || IsNewPlayer;
            }
        }

        /// <summary>
        /// States the inital values of the database values when loaded/saved. The username is ignored because it never changes in the MySql database.
        /// </summary>
        private bool initialIsAdministrator, initialIsBanned;
        private DateTime initialLastVisitDate;
        private PlayerGameStatistics initalStatistics;

        /// <summary>
        /// Constructor for creating a <see cref="PlayersTableRow"/> object which holds data about a player from the <see cref="PlayersTable"/>.
        /// </summary>
        /// <param name="username">Refer to <see cref="Username"/> for description.</param>
        /// <param name="isAdministrator">Refer to <see cref="IsAdministrator"/> for description.</param>
        /// <param name="isNewPlayer">Refer to <see cref="IsNewPlayer"/> for description.</param>
        public PlayersTableRow(string username, DateTime lastVisitDate, bool isAdministrator, bool isBanned, PlayerGameStatistics statistics, bool isNewPlayer)
        {
            Username = username;
            LastVisitDate = lastVisitDate;
            IsAdministrator = isAdministrator;
            Statistics = statistics;
            IsBanned = isBanned;
            IsNewPlayer = isNewPlayer;

            UpdateChanges();
        }

        /// <summary>
        /// Updates the inital value variables for wins, losses, kills, and coins. This method should only be called if the current players data has been saved to the MySql
        /// database successfully.
        /// </summary>
        public void UpdateChanges()
        {
            initialLastVisitDate = LastVisitDate;
            initialIsAdministrator = IsAdministrator;
            initialIsBanned = IsBanned;
            initalStatistics = Statistics.Clone() as PlayerGameStatistics;
        }
    }
}