// File Name:     CtfGameRound.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.DataStructures
{
    public sealed class CtfGameRound
    {
        /// <summary>
        /// A dictionary that stores the <see cref="Team.Blue"/> and the <see cref="Team.Red"/> scores.
        /// </summary>
        public Dictionary<Team, int> Scores { get; private set; } = new Dictionary<Team, int>()
        {
            { Team.Blue, 0 },
            { Team.Red, 0 },
        };

        /// <summary>
        /// 
        /// </summary>
        public int GameFund { get; private set; } = 0;

        /// <summary>
        /// Gets a <see cref="string"/> containing the <see cref="Team.Blue"/> and <see cref="Team.Red"/> scores.
        /// </summary>
        /// <returns>A <see cref="string"/> in the format of: "Blue: [BLUE_TEAM_SCORE] | Red: [RED_TEAM_SCORE]" (excluding quotes).</returns>
        public string GetScoresString()
        {
            return $"Blue: {Scores[Team.Blue]} | Red: {Scores[Team.Red]}";
        }

        public void End(Team winningTeam)
        {
            CtfBot.SendChatMessage($"Game over! Team {TeamHelper.EnumToString(winningTeam)} has won the game.");

            /*int coinsWon = GetGameFundShare(winningTeam);
            foreach (Player player in JoinedWorld.Players.Values)
            {
                PlayerDatabaseRow playerData = PlayersDatabaseTable.GetRow(player.Username);

                if (playerData != null && player.IsPlayingGame)
                {
                    if (player.Team == winningTeam)
                    {
                        playerData.TotalWins++;
                        playerData.Coins += coinsWon;

                        CaptureTheFlagBot.SendPrivateMessage(player, $"You received {coinsWon} coin{(coinsWon == 1 ? "" : "s")} for winning!");
                    }
                    else
                    {
                        playerData.TotalLosses++;
                    }
                }
            }*/

            CtfBot.ResetLevel();

            PlayersDatabaseTable.Save();
        }

        /// <summary>
        /// Increases the <see cref="GameFund"/> by a certain amount of coins, depending on the reason.
        /// </summary>
        /// <param name="reason">The reason why the game fund is being increased.</param>
        public void IncreaseGameFund(GameFundIncreaseReason reason)
        {
            switch (reason)
            {
                case GameFundIncreaseReason.FlagCaptured:
                    GameFund += 25;
                    break;
                case GameFundIncreaseReason.FlagReturned:
                case GameFundIncreaseReason.FlagTaken:
                    GameFund += 10;
                    break;
                default:
                case GameFundIncreaseReason.PlayerKilledEnemy:
                    GameFund += 5;
                    break;
            }
        }

        /// <summary>
        /// Splits the game fund by dividing it by the total number of players on the winning team, excluding guest players.
        /// </summary>
        /// <param name="winningTeam">The team that won the Capture the Flag game.</param>
        /// <returns>An int that represents the amount of coins each player has won on the winning team.</returns>
        private int GetGameFundShare(Team winningTeam)
        {
            int totalTeamPlayers = (JoinedWorld.Players.Values.Where(player => !player.IsGuest && player.Team == winningTeam)).Count();

            return 0; // GameFund.CoinsRaised / totalTeamPlayers;
        }
    }
}
