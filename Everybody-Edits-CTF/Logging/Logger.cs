// File Name:     Logger.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, June 30, 2020

using System;

namespace Everybody_Edits_CTF.Logging
{
    public static class Logger
    {
        /// <summary>
        /// The text written to the logger.
        /// </summary>
        public static string LogText { get; private set; } = string.Empty;

        /// <summary>
        /// Writes the log in the format: "[TYPE @ TIME_THROW]: MESSAGE" (excluding double quotes).
        /// </summary>
        /// <param name="type">The type of log.</param>
        /// <param name="message">The message of the log.</param>
        public static void WriteLog(LogType type, string message)
        {
            LogText += $"[{LogTypeToString(type)} @ {DateTime.Now.ToString("hh:mm tt")}]: {message}\n";
        }

        /// <summary>
        /// Clears the log text by setting it to empty.
        /// </summary>
        public static void ClearLogs()
        {
            LogText = string.Empty;
        }

        /// <summary>
        /// Convert the LogType enum to a readable string.
        /// </summary>
        /// <param name="type">The LogType enum to be converted to a readable string.</param>
        /// <returns>
        /// If the LogType is EverybodyEditsMessage, "EE_MSG" is returned. If the LogType is Exception, "EXCEPTION" is returned. If the LogType is neither of those,
        /// "UNKNOWN_TYPE" is returned.
        /// </returns>
        private static string LogTypeToString(LogType type)
        {
            switch (type)
            {
                case LogType.DatabaseModifcation:
                    return "DATABASE";
                case LogType.EverybodyEditsMessage:
                    return "EE_MSG";
                case LogType.Exception:
                    return "EXCEPTION";
                default:
                    return "UNKNOWN_TYPE";
            }
        }
    }
}