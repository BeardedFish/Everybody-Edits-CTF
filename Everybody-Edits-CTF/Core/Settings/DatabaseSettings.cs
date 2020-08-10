// File Name:     DatabaseSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, July 4, 2020

namespace Everybody_Edits_CTF.Core.Settings
{
    public static class DatabaseSettings
    {
        /// <summary>
        /// The date time format in the MySql database.
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd";

        /// <summary>
        /// The connection string to access the MySql database via <see cref="MySql.Data.MySqlClient.MySqlConnection"/>.
        /// </summary>
        public static readonly string SqlConnectionString = $"Server={ServerUrl}; Port={Port}; Database={Name}; Uid={Username}; Pwd={Password};";

        /// <summary>
        /// The URL to the MySql database.
        /// </summary>
        private const string ServerUrl = "127.0.0.1";

        /// <summary>
        /// The port to access the MySql database.
        /// </summary>
        private const ushort Port = 3306;

        /// <summary>
        /// The name of the database.
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
    }
}