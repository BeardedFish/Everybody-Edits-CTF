// File Name:     Database.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, June 29, 2020

using Everybody_Edits_CTF.Core.DataStructures;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Data
{
    public static class PlayersDatabaseTable
    {
        public static bool Loaded
        {
            get => Rows != null;
        }

        private const string ServerUrl = "";
        private const string DatabaseUsername = "";
        private const string DatabasePassword = "";
        private const ushort DatabasePort = 0;
        
        private static readonly string SqlConnectionString = $"Server={ServerUrl}; Port={DatabasePort}; Database={DatabaseUsername}; Uid={DatabaseUsername}; Pwd={DatabasePassword};";

        public static List<PlayerDatabaseRow> Rows;

        /// <summary>
        /// Loads the Players table from the MySql database.
        /// </summary>
        public static void Load()
        {
            Rows = new List<PlayerDatabaseRow>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(SqlConnectionString))
                {
                    string query = $"SELECT * FROM Players;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Rows.Add(new PlayerDatabaseRow(reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), false));
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                Rows = null;
            }
        }

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

        public static PlayerDatabaseRow GetPlayerDatabaseRow(string username)
        {
            foreach (PlayerDatabaseRow row in Rows)
            {
                if (string.Equals(username, row.Username, StringComparison.OrdinalIgnoreCase))
                {
                    return row;
                }
            }

            return null;
        }

        public static void AddNewUser(string username)
        {
            Rows.Add(new PlayerDatabaseRow(username, 0, 0, 0, true));
        }

        public static void Save()
        {
            foreach (PlayerDatabaseRow player in Rows)
            {

                try
                {
                    string sqlQuery;

                    if (player.IsNewPlayer)
                    {
                        sqlQuery = $"INSERT INTO Players (Id, Username, TotalWins, TotalLosses, Coins) VALUES (NULL, \"{player.Username}\", {player.TotalWins}, {player.TotalLosses}, {player.Coins});";
                    }
                    else
                    {
                        sqlQuery = $"UPDATE Players SET TotalWins={player.TotalWins}, TotalLosses={player.TotalLosses}, Coins={player.Coins} WHERE Username=\"{player.Username}\";";
                    }

                    using (MySqlConnection connection = new MySqlConnection(SqlConnectionString))
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
                    Console.WriteLine(ex.Message); // TODO: Use Logger.cs class
                }
            }
        }
    }
}