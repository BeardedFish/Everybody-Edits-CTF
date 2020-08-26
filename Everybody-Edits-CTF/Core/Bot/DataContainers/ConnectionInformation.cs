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
        /// The world id that the bot will join.
        /// </summary>
        public string WorldId { get; private set; }

        /// <summary>
        /// Creates a <see cref="ConnectionInformation"/> object which stores information about an Everybody Edits bot.
        /// </summary>
        /// <param name="email">Refer to <see cref="Email"/> for description.</param>
        /// <param name="password">Refer to <see cref="Password"/> for description.</param>
        /// <param name="worldId">Refer to <see cref="WorldId"/> for description.</param>
        public ConnectionInformation(string email, string password, string worldId)
        {
            Email = email;
            Password = password;
            WorldId = worldId;
        }
    }
}