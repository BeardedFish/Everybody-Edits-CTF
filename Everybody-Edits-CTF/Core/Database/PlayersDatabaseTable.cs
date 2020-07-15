// File Name:     Database.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Logging;
using Everybody_Edits_CTF.Logging.Enums;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Database
{
    public static class PlayersDatabaseTable
    {
        /// <summary>
        /// The name of the Players table in the MySql database.
        /// </summary>
        private const string PlayersTableName = "Players";

        /// <summary>
        /// States whether the Players table has been loaded or not.
        /// </summary>
        public static bool Loaded
        {
            get => Rows != null;
        }

        /// <summary>
        /// The rows of the Players table loaded from the MySql database.
        /// </summary>
        public static List<PlayerDatabaseRow> Rows { get; private set; }

        /// <summary>
        /// Loads the Players table from the MySql database.
        /// </summary>
        public static void Load()
        {
            Rows = new List<PlayerDatabaseRow>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(DatabaseSettings.SqlConnectionString))
                {
                    string query = $"SELECT * FROM {PlayersTableName};";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Rows.Add(new PlayerDatabaseRow(reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), false));
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
            foreach (PlayerDatabaseRow row in Rows)
            {
                if (row.Username == username)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the PlayerDatabaseRow of a specified player via their username.
        /// </summary>
        /// <param name="username">The username of the player to be searched for in the "Rows" list.</param>
        /// <returns>
        /// If the player is found, then the PlayerDatabaseRow object that correspond to that player is returned. If the player is not found, then null is returned.
        /// </returns>
        public static PlayerDatabaseRow GetPlayerDatabaseRow(string username)
        {
            foreach (PlayerDatabaseRow player in Rows)
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
            Rows.Add(new PlayerDatabaseRow(username, 0, 0, 0, 0, true));
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

            foreach (PlayerDatabaseRow player in Rows)
            {
                try
                {
                    string sqlQuery;

                    if (player.IsNewPlayer)
                    {
                        sqlQuery = $"INSERT INTO {PlayersTableName} (Id, Username, TotalWins, TotalLosses, TotalKills, Coins) VALUES (NULL, \"{player.Username}\", {player.TotalWins}, {player.TotalLosses}, {player.TotalKills}, {player.Coins});";

                        player.IsNewPlayer = false;
                    }
                    else
                    {
                        sqlQuery = $"UPDATE {PlayersTableName} SET TotalWins={player.TotalWins}, TotalLosses={player.TotalLosses}, TotalKills={player.TotalKills}, Coins={player.Coins} WHERE Username=\"{player.Username}\";";
                    }

                    using (MySqlConnection connection = new MySqlConnection(DatabaseSettings.SqlConnectionString))
                    {
                        using (MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, connection))
                        {
                            connection.Open();

                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.WriteLog(LogType.Exception, $"Fail while trying to save the database (reason: {ex.Message}).");
                }
            }
        }
    }
}