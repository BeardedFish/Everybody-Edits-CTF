// File Name:     CaptureTheFlag.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.GameMechanics.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Drawing;
using System.Text;
using System.Linq;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class CaptureTheFlag
    {
        /// <summary>
        /// States whether the blue team's flag has been taken by the red team or not.
        /// </summary>
        public static bool BlueFlagTaken
        {
            get => IsFlagTaken(Team.Blue);
        }
        
        /// <summary>
        /// States whether the red team's flag has been taken by the blue team or not.
        /// </summary>
        public static bool RedFlagTaken
        {
            get => IsFlagTaken(Team.Red);
        }

        /// <summary>
        /// The current score of each team. The teams must reach the max score defined in <see cref="GameSettings.MaxScoreToWin"/> in order to win the game.
        /// </summary>
        public static int BlueTeamScore, RedTeamScore;
        
        /// <summary>
        /// The fund of the current Capture the Flag game going on. The fund is increased every time an event happens in the game such as capturing a flag, taking a flag, etc.
        /// Refer to <see cref="IncreaseGameFund(GameFundIncreaseReason)"/> to see how many coins the game fund increases by on each event. The team that wins the game gets the
        /// fund, where it is divided equally among all the team members.
        /// </summary>
        public static int GameFund;

        /// <summary>
        /// Resets the team scores and the game fund to zero so that a new game can be started.
        /// </summary>
        public static void ResetGameStatistics()
        {
            BlueTeamScore = RedTeamScore = 0;
            GameFund = 0;
        }

        /// <summary>
        /// Increases the <see cref="GameFund"/> by a certain amount of coins, depending on the reason.
        /// </summary>
        /// <param name="reason">The reason why the game fund is being increased.</param>
        public static void IncreaseGameFund(GameFundIncreaseReason reason)
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
        /// Makes a player take the enemy flag if they are able to. If they are not able to take the enemy flag, then this method does nothing.
        /// 
        /// A player is able to take an enemy flag if:
        /// - They are either on the blue team or red team
        /// - They are not in God mode
        /// - The enemy flag is not already taken
        /// - They are located at the enemy flag position
        /// </summary>
        /// <param name="player">The player to take the enemy flag if they are able to.</param>
        public static void TakeFlag(Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode)
            {
                return;
            }

            if (player.Team == Team.Blue && !RedFlagTaken && player.IsOnRedFlag || player.Team == Team.Red && !BlueFlagTaken && player.IsOnBlueFlag)
            {
                player.HasEnemyFlag = true;

                Point enemyFlagLocation = player.Team == Team.Blue ? GameSettings.RedFlagLocation : GameSettings.BlueFlagLocation;        
                CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, enemyFlagLocation, 0);
                
                CaptureTheFlagBot.SendChatMessage($"Player {player.Username} has taken the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag!");

                IncreaseGameFund(GameFundIncreaseReason.FlagTaken);
            }
        }

        /// <summary>
        /// Makes a player capture the enemy flag if they are able to. If they are not able to capture the enemy flag, then this method does nothing.
        /// 
        /// A player is able to capture an enemy flag if:
        /// - They are either on the blue team or red team
        /// - They are not in God mode
        /// - Their team's flag is not taken by the enemy
        /// - They are located at their flag's location
        /// </summary>
        /// <param name="player">The player to capture the enemy flag if they are able to.</param>
        public static void CaptureFlag(Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode)
            {
                return;
            }

            if (player.HasEnemyFlag)
            {
                if (!BlueFlagTaken && player.IsOnBlueFlag || !RedFlagTaken && player.IsOnRedFlag)
                {
                    if (player.Team == Team.Blue)
                    {
                        BlueTeamScore++;
                    }
                    else // Red team
                    {
                        RedTeamScore++;
                    }

                    ReturnFlag(player, false);
                    IncreaseGameFund(GameFundIncreaseReason.FlagCaptured);

                    CaptureTheFlagBot.SendChatMessage($"Player {player.Username} has captured the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag! [B: {BlueTeamScore} | R: {RedTeamScore}]");
                }

                if (BlueTeamScore >= GameSettings.MaxScoreToWin || RedTeamScore >= GameSettings.MaxScoreToWin)
                {
                    GameOver(player.Team);

                    ResetGameStatistics();
                }
            }
        }

        /// <summary>
        /// Returns a team's flag if a player has taken it. If a player died while holding the flag, all the other players in the Everybody Edits world are notified that the
        /// player died and that the flag was returned.
        /// </summary>
        /// <param name="player">The player that is holding the flag to be returned.</param>
        /// <param name="playerDied">States whether the player died in the game or not.</param>
        public static void ReturnFlag(Player player, bool playerDied)
        {
            if (player.HasEnemyFlag)
            {
                player.HasEnemyFlag = false;

                Point enemyFlagLocation = player.Team == Team.Blue ? GameSettings.RedFlagLocation : GameSettings.BlueFlagLocation;
                int flagMorphId = player.Team == Team.Blue ? 4 : 1;
                CaptureTheFlagBot.PlaceBlock(BlockLayer.Foreground, enemyFlagLocation, 327, flagMorphId);

                if (playerDied)
                {
                    StringBuilder teamName = new StringBuilder(TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team)));
                    teamName[0] = char.ToUpper(teamName[0]);

                    CaptureTheFlagBot.SendChatMessage($"Player {player.Username} died! {teamName} flag returned to base.");
                }
            }
        }

        /// <summary>
        /// Ends the game by giving the players on the winning team their fair share of the game fund as well as incrementing their total wins. Players on the losing team will have
        /// their total losses incremented.
        /// </summary>
        /// <param name="winningTeam">The team that won the Capture the Flag game.</param>
        public static void GameOver(Team winningTeam)
        {
            int coinsWon = GetGameFundShare(winningTeam);

            foreach (Player player in CaptureTheFlagBot.PlayersInWorld.Values)
            {
                PlayerDatabaseRow playerData = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);

                if (playerData == null || !player.IsPlayingGame)
                {
                    continue;
                }

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

            CaptureTheFlagBot.ResetLevel();
            CaptureTheFlagBot.SendChatMessage($"Game over! Team {TeamHelper.EnumToString(winningTeam)} has won the game.");

            PlayersDatabaseTable.Save();
        }

        /// <summary>
        /// Splits the game fund by dividing it by the total number of players on the winning team.
        /// </summary>
        /// <param name="winningTeam">The team that won the Capture the Flag game.</param>
        /// <returns>An int that represents the amount of coins each player has won on the winning team.</returns>
        private static int GetGameFundShare(Team winningTeam)
        {
            int totalTeamPlayers = (CaptureTheFlagBot.PlayersInWorld.Values.Where(player => player.Team == winningTeam)).Count();
            
            return GameFund / totalTeamPlayers;
        }

        /// <summary>
        /// States whether a team's flag has been taken by the enemy team or not.
        /// </summary>
        /// <param name="team">The team to see if their flag was taken or not.</param>
        /// <returns>True if the team's flag was taken by the enemy team, if not, false.</returns>
        private static bool IsFlagTaken(Team team)
        {
            foreach (Player player in CaptureTheFlagBot.PlayersInWorld.Values)
            {
                if (TeamHelper.GetOppositeTeam(player.Team) == team && player.HasEnemyFlag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}