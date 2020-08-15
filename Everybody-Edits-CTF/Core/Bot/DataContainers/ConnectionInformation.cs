// File Name:     ConnectionInformation.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, August 14, 2020

namespace Everybody_Edits_CTF.Core.Bot.DataContainers
{
    public sealed class ConnectionInformation
    {
        /// <summary>
        /// The email of the bot account.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// The password of the bot account.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// The world id of the bot account.
        /// </summary>
        public string WorldId { get; private set; }

        /// <summary>
        /// Creates a <see cref="ConnectionInformation"/> object which stores information about an Everybody Edits bot.
        /// </summary>
        /// <param name="email">The email of the bot.</param>
        /// <param name="password">The password of the bot.</param>
        /// <param name="worldId">The world id of the bot.</param>
        public ConnectionInformation(string email, string password, string worldId)
        {
            Email = email;
            Password = password;
            WorldId = worldId;
        }
    }
}