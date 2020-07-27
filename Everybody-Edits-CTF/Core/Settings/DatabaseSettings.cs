// File Name:     DatabaseSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, July 4, 2020

namespace Everybody_Edits_CTF.Core.Settings
{
    public static class DatabaseSettings
    {
        /// <summary>
        /// The URL to the MySql database.
        /// </summary>
        public const string ServerUrl = "";

        /// <summary>
        /// The port to access the MySql database.
        /// </summary>
        public const ushort DatabasePort = 0;

        /// <summary>
        /// The username for the MySql database.
        /// </summary>
        public const string DatabaseUsername = "";

        /// <summary>
        /// The password for the MySql database.
        /// </summary>
        public const string DatabasePassword = "";

        /// <summary>
        /// The connection string to access the MySql database via <see cref="MySql.Data.MySqlClient.MySqlConnection"/>.
        /// </summary>
        public static readonly string SqlConnectionString = $"Server={ServerUrl}; Port={DatabasePort}; Database={DatabaseUsername}; Uid={DatabaseUsername}; Pwd={DataabasePassword};";
    }
}