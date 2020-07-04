// File Name:     GameSettings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot
{
    public static class GameSettings
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
        public const string BotEmail = "";

        /// <summary>
        /// The password of the bot account.
        /// </summary>
        public const string BotPassword = "";

        /// <summary>
        /// States whether the bot will auto balance teams if they are unbalanced.
        /// </summary>
        public const bool AutoBalanceTeams = true;

        /// <summary>
        /// States the max score a team needs to achieve in order to win the game.
        /// </summary>
        public const ushort MaxScoreToWin = 5;

        /// <summary>
        /// The location of the red teams flag in the world that the bot joined.
        /// </summary>
        public static readonly Point BlueFlagLocation = new Point(32, 144);
        
        /// <summary>
        /// The location of the red teams flag in the world that the bot joined.
        /// </summary>
        public static readonly Point RedFlagLocation = new Point(367, 144);
    }
}