// File Name:     BotSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 6, 2020

namespace Everybody_Edits_CTF.Core.Settings
{
    public static class BotSettings
    {
        /// <summary>
        /// States the usernames of players who are administrators in the Everybody Edits world.
        /// </summary>
        public static readonly string[] Administrators = { "" };

        /// <summary>
        /// The chat message prefix for public messages sent by the bot.
        /// </summary>
        public const string ChatMessagePrefix = "[CTF_BOT]:";

        /// <summary>
        /// The game ID for Everybody Edits.
        /// </summary>
        public const string EverybodyEditsGameId = "everybody-edits-su9rn58o40itdbnw69plyw";

        /// <summary>
        /// The id of the Everybody Edits world that the bot will join.
        /// </summary>
        public const string WorldId = "";

        /// <summary>
        /// The email of the bot account.
        /// </summary>
        public const string Email = "";

        /// <summary>
        /// The password of the bot account.
        /// </summary>
        public const string Password = "";
        /// <summary>
        /// The title the bot will set the Everybody Edits world to when connected.
        /// </summary>
        public const string WorldTitle = "CTF Bot";
    }
}