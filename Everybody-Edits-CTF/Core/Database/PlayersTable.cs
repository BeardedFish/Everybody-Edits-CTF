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
        private const string PlayersTableName = "players";

        /// <summary>
        /// The name of the table which contains game statistics about a player.
        /// </summary>
        private const string GameStatisticsTableName = "player_game_stats";

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
                    string query = $"SELECT Username, LastVisitDate, IsAdministrator, TotalWins, TotalLosses, TotalKills, Coins FROM {PlayersTableName} INNER JOIN {GameStatisticsTableName} ON Id = PlayerId;";

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
                                statistics,
                                false);

                            Rows.Add(playerData);
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
            Rows.Add(new PlayersTableRow(username, DateTime.Today, false, new PlayerGameStatistics(), true));
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

            using (MySqlConnection connection = new MySqlConnection(DatabaseSettings.SqlConnectionString))
            {
                connection.Open();

                foreach (PlayersTableRow playerData in Rows)
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
                            queries.Add(new MySqlCommand($"INSERT INTO {PlayersTableName} (Id, Username, LastVisitDate, IsAdministrator) VALUES (NULL, \"{playerData.Username}\", {playerData.LastVisitDate.ToString(DatabaseSettings.DateTimeFormat)}, {Convert.ToInt32(playerData.IsAdministrator)});", connection));
                            queries.Add(new MySqlCommand($"INSERT INTO {GameStatisticsTableName} (Id, TotalWins, TotalLosses, TotalKills, Coins) VALUES ((SELECT Id FROM {PlayersTableName} WHERE Username=\"{playerData.Username}\" LIMIT 1), {playerData.Statistics.TotalWins}, {playerData.Statistics.TotalLosses}, {playerData.Statistics.TotalKills}, {playerData.Statistics.Coins});", connection));

                            playerData.IsNewPlayer = false;
                        }
                        else
                        {
                            queries.Add(new MySqlCommand($"UPDATE {PlayersTableName} SET LastVisitDate=\"{playerData.LastVisitDate.ToString(DatabaseSettings.DateTimeFormat)}\", IsAdministrator={Convert.ToInt32(playerData.IsAdministrator)} WHERE Username=\"{playerData.Username}\";", connection));
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

                Logger.WriteLog(LogType.DatabaseModifcation, $"Total MySql database entries inserted/modified: {totalDatabaseModifications}).");
            }
        }
    }
}