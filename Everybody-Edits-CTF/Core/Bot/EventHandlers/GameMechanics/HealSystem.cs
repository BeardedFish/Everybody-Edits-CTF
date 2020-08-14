// File Name:     HealSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class HealSystem : IGameMechanic
    {
        public void Handle(string messageType, Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode || player.SmileyId != (int)Smiley.Nurse)
            {
                return;
            }

            foreach (Player allyPlayer in JoinedWorld.Players.Values)
            {
                if (player == allyPlayer)
                {
                    continue;
                }

                if (!player.IsEnemiesWith(allyPlayer) && player.IsNearPlayer(allyPlayer) && allyPlayer.Health < 100)
                {
                    if (allyPlayer.Heal())
                    {
                        CtfBot.SendPrivateMessage(allyPlayer, $"You were healed player {player.Username}");
                        CtfBot.SendPrivateMessage(player, $"You fully healed player {allyPlayer.Username}");
                    }
                }
            }
        }
    }
}