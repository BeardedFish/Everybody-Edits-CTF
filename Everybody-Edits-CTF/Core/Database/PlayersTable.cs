// File Name:     PlayersTable.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Database
{
    public static class PlayersTable
    {
        /// <summary>
        /// States whether the Players table has been loaded or not.
        /// </summary>
        public static bool Loaded => rows != null;

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

        /// <summary>
        /// The rows of the Players table loaded from the MySql database.
        /// </summary>
        private static List<PlayersTableRow> rows;

        /// <summary>
        /// Loads the Players table from the MySql database.
        /// </summary>
        public static void Load()
        {
            rows = new List<PlayersTableRow>();

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
                            PlayerGameStatistics statistics = new PlayerGameStatistics(sqlReader.GetInt32(3),
                                sqlReader.GetInt32(4),
                                sqlReader.GetInt32(5),
                                sqlReader.GetInt32(6));

                            PlayersTableRow playerData = new PlayersTableRow(sqlReader.GetString(0),
                                sqlReader.GetDateTime(1),
                                sqlReader.GetBoolean(2),
                                sqlReader.GetBoolean(3),
                                statistics,
                                false);

                            rows.Add(playerData);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                rows = null;

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
        /// Gets the <see cref="PlayersTableRow"/> of a specified player via their username.
        /// </summary>
        /// <param name="username">The username of the player to be searched for in the "Rows" list.</param>
        /// <returns>
        /// If the player is found, then the <see cref="PlayersTableRow"/> object that correspond to that player is returned. If the player is not found, then null is
        /// returned.
        /// </returns>
        public static PlayersTableRow GetRow(string username)
        {
            if (Loaded)
            {
                foreach (PlayersTableRow player in rows)
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
        public static void AddNewPlayer(string username)
        {
            rows.Add(new PlayersTableRow(username, DateTime.Today, false, false, new PlayerGameStatistics(), true));
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

            using (MySqlConnection connection = new MySqlConnection(SqlConnectionString))
            {
                connection.Open();

                foreach (PlayersTableRow playerData in rows)
                {
                    if (!playerData.ChangesOccured)
                    {
                        continue;
                    }

                    try
                    {
                        List<MySqlCommand> queries = new List<MySqlCommand>();

                        if (playerData.IsNewPlayer)
                        {
                            queries.Add(new MySqlCommand($"INSERT INTO {PlayersTableName} (Id, Username, LastVisitDate, IsAdministrator, IsBanned) VALUES (NULL, \"{playerData.Username}\", \"{playerData.LastVisitDate.ToString(DateTimeFormat)}\", {Convert.ToInt32(playerData.IsAdministrator)}, {Convert.ToInt32(playerData.IsBanned)});", connection));
                            queries.Add(new MySqlCommand($"INSERT INTO {GameStatisticsTableName} (PlayerId, TotalWins, TotalLosses, TotalKills, Coins) VALUES ((SELECT Id FROM {PlayersTableName} WHERE Username=\"{playerData.Username}\" LIMIT 1), {playerData.Statistics.TotalWins}, {playerData.Statistics.TotalLosses}, {playerData.Statistics.TotalKills}, {playerData.Statistics.Coins});", connection));

                            playerData.IsNewPlayer = false;
                        }
                        else
                        {
                            queries.Add(new MySqlCommand($"UPDATE {PlayersTableName} SET LastVisitDate=\"{playerData.LastVisitDate.ToString(DateTimeFormat)}\", IsAdministrator={Convert.ToInt32(playerData.IsAdministrator)} WHERE Username=\"{playerData.Username}\";", connection));
                            queries.Add(new MySqlCommand($"UPDATE {GameStatisticsTableName} SET TotalWins={playerData.Statistics.TotalWins}, TotalLosses={playerData.Statistics.TotalLosses}, TotalKills={playerData.Statistics.TotalKills}, Coins={playerData.Statistics.Coins} WHERE PlayerId=(SELECT Id FROM {PlayersTableName} WHERE Username=\"{playerData.Username}\" LIMIT 1);", connection));
                        }

                        // Run all SQL queries
                        foreach (MySqlCommand sqlQuery in queries)
                        {
                            sqlQuery.ExecuteNonQuery();
                            playerData.UpdateChanges();

                            sqlQuery.Dispose();
                        }

                        totalDatabaseModifications++;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.WriteLog(LogType.Exception, $"Fail while trying to save the database (reason: {ex.Message}).");
                    }
                }
            }

            Logger.WriteLog(LogType.DatabaseModifcation, $"Total MySql database entries inserted/modified: {totalDatabaseModifications}).");
        }
    }
}