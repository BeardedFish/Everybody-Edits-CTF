// File Name:     KillCredit.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventArgs;
using Everybody_Edits_CTF.Core.Database;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class KillCredit
    {
        /// <summary>
        /// Game mechanic which handles kill credit in the Capture The Flag game. Both the attacker and the attackee are notifed about the event via a private message.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public KillCredit(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnPlayerReset += OnPlayerReset;
        }

        /// <summary>
        /// Credits a player if they succesfully kill another player during the Capture The Flag game.
        /// 
        /// NOTE: If the <see cref="MySqlDatabase"/> is not loaded then the kill is not accumulated to the attackers total kill count.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="eventArgs">The arguments for when the player (or players) was/were reset.</param>
        private void OnPlayerReset(CaptureTheFlagBot ctfBot, PlayerResetEventArgs eventArgs)
        {
            if (eventArgs.PlayersReset.Count == 1)
            {
                Player killedPlayer = eventArgs.PlayersReset[0];

                if (killedPlayer.LastAttacker != Player.None)
                {
                    ctfBot.SendPrivateMessage(killedPlayer, $"You were killed by player {killedPlayer.LastAttacker.Username}!");
                    ctfBot.SendPrivateMessage(killedPlayer.LastAttacker, $"You killed player {killedPlayer.Username}!");

                    if (MySqlDatabase.Loaded)
                    {
                        PlayerData playerData = MySqlDatabase.GetRow(killedPlayer.LastAttacker.Username);

                        if (playerData != null)
                        {
                            playerData.Statistics.TotalKills++;
                        }
                    }

                    ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.PlayerKilledEnemy);
                }

                killedPlayer.LastAttacker = Player.None;
            }
        }
    }
}