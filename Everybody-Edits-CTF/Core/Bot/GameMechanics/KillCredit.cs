// File Name:     KillCredit.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventArgs;
using Everybody_Edits_CTF.Core.Database;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class KillCredit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, PlayerResetEventArgs eventArgs)
        {
            if (eventArgs.PlayersReset.Count == 1)
            {
                Player killedPlayer = eventArgs.PlayersReset[0];

                if (killedPlayer.LastAttacker != null)
                {
                    ctfBot.SendPrivateMessage(killedPlayer, $"You were killed by player {killedPlayer.LastAttacker.Username}!");
                    ctfBot.SendPrivateMessage(killedPlayer.LastAttacker, $"You killed player {killedPlayer.Username}!");

                    if (PlayersTable.Loaded)
                    {
                        PlayerRow playerData = PlayersTable.GetRow(killedPlayer.LastAttacker.Username);

                        if (playerData != null)
                        {
                            playerData.TotalKills++;
                        }
                    }

                    ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.PlayerKilledEnemy);
                }

                killedPlayer.LastAttacker = null;
            }
        }
    }
}