﻿// File Name:     IGameMechanic.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public interface IGameMechanic
    {
        /// <summary>
        /// Handles a specific game mechanic for the Capture The Flag game. Implementation will vary.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        void Handle(CtfBot ctfBot, string messageType, Player player);
    }
}