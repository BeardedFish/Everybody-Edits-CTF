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
        /// Credits a player if they succesfully kill another player during the Capture The Flag game. This method is called each time a player is reset in the Everybody Edits world. Both
        /// the attacker and the attackee are notifed about the kill credit via a private message.
        /// 
        /// NOTE: If the <see cref="PlayersTable"/> is not loaded then the kill is not accumulated to the attackers total kill count.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="eventArgs">The arguments for when the player (or players) was/were reset.</param>
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
                        PlayersTableRow playerData = PlayersTable.GetRow(killedPlayer.LastAttacker.Username);

                        if (playerData != null)
                        {
                            playerData.Statistics.TotalKills++;
                        }
                    }

                    ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.PlayerKilledEnemy);
                }

                killedPlayer.LastAttacker = null;
            }
        }
    }
}