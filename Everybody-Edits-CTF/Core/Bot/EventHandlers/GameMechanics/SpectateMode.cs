// File Name:     SpectateMode.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class SpectateMode : IGameMechanic
    {
        /// <summary>
        /// Handles when a player enters/leaves spectate mode. The command implementation is defined in the <see cref="RegularCommands.cs"/> file.
        /// </summary>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(string messageType, Player player)
        {
            if (!player.CanToggleGodMode)
            {
                if (!player.IsInGodMode)
                {
                    player.GoToLobby();
                }

                CtfBot.SendPrivateMessage(player, player.IsInGodMode ? "You have entered spectate mode. Type !spectate again to exit out of spectate mode." : "You have left spectate mode.");
            }
        }
    }
}