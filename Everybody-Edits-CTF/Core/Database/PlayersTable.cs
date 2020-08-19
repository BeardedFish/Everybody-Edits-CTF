// File Name:     PlayersTable.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Database
{
    public static class PlayersTable
    {
        /// <summary>
        /// The name of the Players table in the MySql database.
        /// </summary>
        private const string Name = "players";

        /// <summary>
        /// States whether the Players table has been loaded or not.
        /// </summary>
        public static bool Loaded => Rows != null;

        /// <summary>
        /// The rows of the Players table loaded from the MySql database.
        /// </summary>
        public static List<PlayersTableRow> Rows { get; private set; }

        /// <summary>
        /// Loads the Players table from the MySql database.
        /// </summary>
        public static void Load()
        {
            Rows = new List<PlayersTableRow>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(DatabaseSettings.SqlConnectionString))
                {
                    string query = $"SELECT * FROM {Name};";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Rows.Add(new PlayersTableRow(reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetDateTime(6), false));
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Rows = null;

                Logger.WriteLog(LogType.Exception, $"Exception caught while trying to load the database (message: {ex.Message}).");
            }
        }

        /// <summary>
        /// Returns a boolean on whether a player exists in the "Rows" list or not.
        /// </summary>
        /// <param name="username">The username of the player.</param>
        /// <returns>True if the player exists in the "Rows" list, if not, false.</returns>
        public static bool PlayerExists(string username)
        {
            foreach (PlayersTableRow row in Rows)
            {
                if (username.Equals(row.Username, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="PlayersTableRow"/> of a specified player via their username.
        /// </summary>
        /// <param name="username">The username of the player to be searched for in the "Rows" list.</param>
        /// <returns>
        /// If the player is found, then the <see cref="PlayersTableRow"/> object that correspond to that player is returned. If the player is not found, then null is
        /// returned.
        /// </returns>
        public static PlayersTableRow GetRow(string username)
        {
            foreach (PlayersTableRow player in Rows)
            {
                if (string.Equals(username, player.Username, StringComparison.OrdinalIgnoreCase))
                {
                    return player;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a new player to the "Rows" list with default data.
        /// </summary>
        /// <param name="username">The username of the player to be added.</param>
        public static void AddNewPlayer(string username)
        {
            Rows.Add(new PlayersTableRow(username, 0, 0, 0, 0, DateTime.Today, true));
        }

        /// <summary>
        /// Saves all data in the "Rows" list by uploading it to the MySql database.
        /// </summary>
        public static void Save()
        {
            if (!Loaded)
            {
                return;
            }

            int totalDatabaseModifications = 0;
            foreach (PlayersTableRow playerData in Rows)
            {
                if (!playerData.ChangesOccured)
                {
                    continue;
                }

                try
                {
                    string sqlQuery;

                    if (playerData.IsNewPlayer)
                    {
                        // NOTE: For the SQL query, the "LastVisitDate" column in the players table will use the default value
                        sqlQuery = $"INSERT INTO {Name} (Id, Username, TotalWins, TotalLosses, TotalKills, Coins) VALUES (NULL, \"{playerData.Username}\", {playerData.TotalWins}, {playerData.TotalLosses}, {playerData.TotalKills}, {playerData.Coins});";
                        playerData.IsNewPlayer = false;
                    }
                    else
                    {
                        sqlQuery = $"UPDATE {Name} SET TotalWins={playerData.TotalWins}, TotalLosses={playerData.TotalLosses}, TotalKills={playerData.TotalKills}, Coins={playerData.Coins}, LastVisitDate=\"{playerData.LastVisitDate.ToString(DatabaseSettings.DateTimeFormat)}\" WHERE Username=\"{playerData.Username}\";";
                    }

                    using (MySqlConnection connection = new MySqlConnection(DatabaseSettings.SqlConnectionString))
                    {
                        using (MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, connection))
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();

                            playerData.UpdateChanges();

                            totalDatabaseModifications++;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.WriteLog(LogType.Exception, $"Fail while trying to save the database (reason: {ex.Message}).");
                }
            }

            Logger.WriteLog(LogType.DatabaseModifcation, $"Total MySql database entries inserted/modified: {totalDatabaseModifications}).");
        }
    }
}