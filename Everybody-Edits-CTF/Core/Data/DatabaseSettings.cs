// File Name:     DatabaseSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, July 4, 2020

namespace Everybody_Edits_CTF.Core.Data
{
    public static class DatabaseSettings
    {
        public const string ServerUrl = "";
        public const string DatabaseUsername = "";
        public const string DatabasePassword = "";
        public const ushort DatabasePort = 0;

        public static readonly string SqlConnectionString = $"Server={ServerUrl}; Port={DatabasePort}; Database={DatabaseUsername}; Uid={DatabaseUsername}; Pwd={DatabasePassword};";
    }
}