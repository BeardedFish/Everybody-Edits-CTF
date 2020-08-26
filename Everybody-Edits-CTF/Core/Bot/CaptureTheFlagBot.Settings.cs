// File Name:     CaptureTheFlagBot.Settings.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, August 25, 2020

using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot
{
    public partial class CaptureTheFlagBot
    {
        /// <summary>
        /// States whether the bot should auto reconnect when disconnected due to a server side failure.
        /// </summary>
        public const bool AutoReconnectOnDisconnect = true;

        /// <summary>
        /// The chat message prefix for public messages sent by the bot.
        /// </summary>
        public const string ChatMessagePrefix = "[CTF]:";

        /// <summary>
        /// The game ID for Everybody Edits.
        /// </summary>
        public const string EverybodyEditsGameId = "everybody-edits-su9rn58o40itdbnw69plyw";

        /// <summary>
        /// The title the bot will set the Everybody Edits world to when connected.
        /// </summary>
        public const string WorldTitle = "CTF Bot";

        /// <summary>
        /// The location where the bot will be moved to when joining the Everybody Edits world.
        /// </summary>
        public static readonly Point JoinLocation = new Point(0, 0);
    }
}