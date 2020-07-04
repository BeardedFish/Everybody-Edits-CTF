// File Name:     Logger.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, June 30, 2020


namespace Everybody_Edits_CTF.Logging
{
    public static class Logger
    {
        private static string logText;

        public static void WriteLog(string message)
        {
            logText += message + "\n";
        }

        public static void ClearLogs()
        {
            logText = string.Empty;
        }
    }
}
