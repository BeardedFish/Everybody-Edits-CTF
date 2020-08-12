﻿// File Name:     CaptureTheFlag.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class CaptureTheFlag
    {
        /// <summary>
        /// A dictionary that contains the team flags for the Capture The Flag game. To access a specific teams flag, use the <see cref="Team"/> enum as the key.
        /// </summary>
        public static Dictionary<Team, Flag> Flags = new Dictionary<Team, Flag>()
        {
            { Team.Blue, new Flag(Team.Blue, GameSettings.BlueFlagLocation, new MorphableBlock(327, 1)) },
            { Team.Red, new Flag(Team.Red, GameSettings.RedFlagLocation, new MorphableBlock(327, 4)) },
        };

        /// <summary>
        /// A dictionary that stores the <see cref="Team.Blue"/> and the <see cref="Team.Red"/> scores.
        /// </summary>
        public static Dictionary<Team, int> Scores = new Dictionary<Team, int>()
        {
            { Team.Blue, 0 },
            { Team.Red, 0 },
        };

        /// <summary>
        /// Handles events in the Capture The Flag game such as a player taking, returning, and capturing a flag.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(Player player)
        {
            if (player.IsInGodMode || !player.IsPlayingGame)
            {
                return;
            }

            Flag friendlyFlag = Flags[player.Team];
            Flag enemyFlag = Flags[TeamHelper.GetOppositeTeam(player.Team)];

            if (enemyFlag.CanBeTakenBy(player))
            {
                enemyFlag.Take(player);

                GameFund.Increase(GameFundIncreaseReason.FlagTaken);
            }

            if (friendlyFlag.CanBeReturnedBy(player))
            {
                friendlyFlag.Return(player, false);
            }

            if (enemyFlag.Holder == player
                && !friendlyFlag.IsDropped
                && !friendlyFlag.IsTaken
                && player.Location == friendlyFlag.HomeLocation)
            {
                enemyFlag.Capture();
                
                Scores[player.Team]++;

                CaptureTheFlagBot.SendChatMessage(GetScoresString());
                GameFund.Increase(GameFundIncreaseReason.FlagCaptured);

                if (Scores[player.Team] >= GameSettings.MaxScoreToWin)
                {
                    GameOver(player.Team);

                    ResetGameStatistics();
                }
            }
        }

        /// <summary>
        /// Resets the team scores and the game fund to zero so that a new game can be started.
        /// </summary>
        public static void ResetGameStatistics()
        {
            Scores[Team.Blue] = Scores[Team.Red] = 0;
            GameFund.CoinsRaised = 0;
        }

        /// <summary>
        /// Gets a <see cref="string"/> containing the <see cref="Team.Blue"/> and <see cref="Team.Red"/> scores.
        /// </summary>
        /// <returns>A <see cref="string"/> in the format of: "Blue: [BLUE_TEAM_SCORE] | Red: [RED_TEAM_SCORE]" (excluding quotes).</returns>
        public static string GetScoresString()
        {
            return $"Blue: {Scores[Team.Blue]} | Red: {Scores[Team.Red]}";
        }

        /// <summary>
        /// Splits the game fund by dividing it by the total number of players on the winning team, excluding guest players.
        /// </summary>
        /// <param name="winningTeam">The team that won the Capture the Flag game.</param>
        /// <returns>An int that represents the amount of coins each player has won on the winning team.</returns>
        private static int GetGameFundShare(Team winningTeam)
        {
            int totalTeamPlayers = (JoinedWorld.Players.Values.Where(player => !player.IsGuest && player.Team == winningTeam)).Count();

            return GameFund.CoinsRaised / totalTeamPlayers;
        }

        /// <summary>
        /// Ends the game by giving the players on the winning team their fair share of the game fund as well as incrementing their total wins. Players on the losing team will have
        /// their total losses incremented.
        /// </summary>
        /// <param name="winningTeam">The team that won the Capture the Flag game.</param>
        private static void GameOver(Team winningTeam)
        {
            CaptureTheFlagBot.SendChatMessage($"Game over! Team {TeamHelper.EnumToString(winningTeam)} has won the game.");

            int coinsWon = GetGameFundShare(winningTeam);
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
            }

            CaptureTheFlagBot.ResetLevel();

            PlayersDatabaseTable.Save();
        }
    }
}