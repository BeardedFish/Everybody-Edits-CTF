// File Name:     IGameMechanic.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public interface IGameMechanic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="player"></param>
        public void Handle(string messageType, Player player);
    }
}