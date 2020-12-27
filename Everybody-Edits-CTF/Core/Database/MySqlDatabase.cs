// File Name:     MySqlDatabase.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Database
{
    public static class MySqlDatabase
    {
        /// <summary>
        /// States whether the Players table has been loaded or not.
        /// </summary>
        public static bool Loaded => m_rows != null;

        /// <summary>
        /// The date time format in the MySql database.
        /// </summary>
        private const string DateTimeFormat = "yyyy-MM-dd";

        /// <summary>
        /// The URL to the MySql database.
        /// </summary>
        private const string ServerUrl = "127.0.0.1";

        /// <summary>
        /// The port to access the MySql database.
        /// </summary>
        private const ushort Port = 3306;

        /// <summary>
        /// The name of the database that contains the tables.
        /// </summary>
        private const string Name = "everybody_edits_ctf";

        /// <summary>
        /// The username for the MySql database.
        /// </summary>
        private const string Username = "root";

        /// <summary>
        /// The password for the MySql database.
        /// </summary>
        private const string Password = "";

        /// <summary>
        /// The name of the Players table in the MySql database.
        /// </summary>
        private const string PlayersTableName = "players";

        /// <summary>
        /// The name of the table which contains game statistics about a player.
        /// </summary>
        private const string GameStatisticsTableName = "player_game_stats";

        /// <summary>
        /// The connection string to access the MySql database via <see cref="MySqlConnection"/>.
        /// </summary>
        private static readonly string SqlConnectionString = $"Server={ServerUrl}; Port={Port}; Database={Name}; Uid={Username}; Pwd={Password};";

#nullable enable
        /// <summary>
        /// The rows of the Players table loaded from the MySql database.
        /// </summary>
        private static List<PlayerData>? m_rows;
#nullable disable

        /// <summary>
        /// Loads the Players table from the MySql database.
        /// </summary>
        public static void Load()
        {
            m_rows = new List<PlayerData>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(SqlConnectionString))
                {
                    string query = $"SELECT Username, LastVisitDate, IsAdministrator, IsBanned, TotalWins, TotalLosses, TotalKills, Coins FROM {PlayersTableName} INNER JOIN {GameStatisticsTableName} ON Id = PlayerId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        MySqlDataReader sqlReader = cmd.ExecuteReader();
                        while (sqlReader.Read())
                        {
                            PlayerGameStatistics statistics = new PlayerGameStatistics(sqlReader.GetInt32(4),
                                sqlReader.GetInt32(5),
                                sqlReader.GetInt32(6),
                                sqlReader.GetInt32(7));

                            PlayerData playerData = new PlayerData(sqlReader.GetString(0),
                                sqlReader.GetDateTime(1),
                                sqlReader.GetBoolean(2),
                                sqlReader.GetBoolean(3),
                                statistics,
                                false);

                            m_rows.Add(playerData);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                m_rows = null;

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
            return GetRow(username) != null;
        }

        /// <summary>
        /// Gets the <see cref="PlayerData"/> of a specified player via their username.
        /// </summary>
        /// <param name="username">The username of the player to be searched for in the "Rows" list.</param>
        /// <returns>
        /// If the player is found, then the <see cref="PlayerData"/> object that correspond to that player is returned. If the player is not found, then null is
        /// returned.
        /// </returns>
        public static PlayerData GetRow(string username)
        {
            if (Loaded)
            {
                foreach (PlayerData player in m_rows)
                {
                    if (string.Equals(username, player.Username, StringComparison.OrdinalIgnoreCase))
                    {
                        return player;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a new player to the "Rows" list with default data.
        /// </summary>
        /// <param name="username">The username of the player to be added.</param>
        /// <param name="isBanned">States whether this player is banned from the Everybody Edits world or not.</param>
        public static void AddNewPlayer(string username, bool isBanned)
        {
            m_rows.Add(new PlayerData(username, DateTime.Today, false, isBanned, new PlayerGameStatistics(), true));
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

            foreach (PlayerData playerData in m_rows)
            {
                if (!playerData.ChangesOccured)
                {
                    continue;
                }

                try
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(SqlConnectionString))
                    {
                        mySqlConnection.Open();

                        // BEGIN UPDATING PLAYERS TABLE //

                        string sqlQuery = playerData.IsNewPlayer
                            ? $"INSERT INTO {PlayersTableName} (Id, Username, LastVisitDate, IsAdministrator, IsBanned) VALUES(NULL, @username, @lastVisitDate, @isAdmin, @isBanned);"
                            : $"UPDATE {PlayersTableName} SET LastVisitDate=@lastVisitDate, IsAdministrator=@isAdmin, IsBanned=@isBanned WHERE Username=@username;";

                        using (MySqlCommand query = new MySqlCommand(sqlQuery, mySqlConnection))
                        {
                            query.Parameters.AddWithValue("@username", playerData.Username);
                            query.Parameters.AddWithValue("@lastVisitDate", playerData.LastVisitDate.ToString(DateTimeFormat));
                            query.Parameters.AddWithValue("@isAdmin", playerData.IsAdministrator);
                            query.Parameters.AddWithValue("@isBanned", playerData.IsBanned);
                            query.ExecuteNonQuery();
                        }

                        //  END UPDATING PLAYERS TABLE  //

                        // BEGIN UPDATING PLAYER STATISTICS TABLE //

                        sqlQuery = playerData.IsNewPlayer
                            ? $"INSERT INTO {GameStatisticsTableName} (PlayerId, TotalWins, TotalLosses, TotalKills, Coins) VALUES ((SELECT Id FROM {PlayersTableName} WHERE Username=@username LIMIT 1), ?totalWins, ?totalLosses, ?totalKills, ?coins);"
                            : $"UPDATE { GameStatisticsTableName} SET TotalWins = @totalWins, TotalLosses = @totalLosses, TotalKills = @totalKills, Coins = @coins WHERE PlayerId = (SELECT Id FROM {PlayersTableName} WHERE Username = @username LIMIT 1);";

                        using (MySqlCommand query = new MySqlCommand(sqlQuery, mySqlConnection))
                        {
                            query.Parameters.AddWithValue("@username", playerData.Username);
                            query.Parameters.AddWithValue("@totalWins", playerData.Statistics.TotalWins);
                            query.Parameters.AddWithValue("@totalLosses", playerData.Statistics.TotalLosses);
                            query.Parameters.AddWithValue("@totalKills", playerData.Statistics.TotalKills);
                            query.Parameters.AddWithValue("@coins", playerData.Statistics.Coins);
                            query.ExecuteNonQuery();
                        }

                        //  END UPDATING PLAYER STATISTICS TABLE  //
                    }

                    playerData.IsNewPlayer = false;
                    playerData.UpdateChanges();

                    totalDatabaseModifications++;
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