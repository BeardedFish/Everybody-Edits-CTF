// File Name:     CaptureTheFlag.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Data;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.GameMechanics.Enums;
using Everybody_Edits_CTF.Enums;
using Everybody_Edits_CTF.Helpers;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class CaptureTheFlag
    {
        public static bool BlueFlagTaken, RedFlagTaken;
        public static int BlueTeamScore, RedTeamScore;
        public static int GameFund;

        public static void IncreaseGameFund(GameFundIncreaseReason reason)
        {
            switch (reason)
            {
                case GameFundIncreaseReason.FlagCaptured:
                    GameFund += 50;
                    break;
                case GameFundIncreaseReason.FlagReturned:
                case GameFundIncreaseReason.FlagTaken:
                    GameFund += 15;
                    break;        
                default:
                case GameFundIncreaseReason.PlayerKilledEnemy:
                    GameFund += 75;
                    break;
            }
        }

        public static void TakeFlag(Player player)
        {
            if (player.IsInGodMode || player.Team == Team.None)
            {
                return;
            }

            if (player.Team == Team.Blue && !RedFlagTaken && player.IsOnRedFlag || player.Team == Team.Red && !BlueFlagTaken && player.IsOnBlueFlag)
            {
                if (player.Team == Team.Blue)
                {
                    RedFlagTaken = true;
                }
                else // Red team
                {
                    BlueFlagTaken = true;
                }

                player.HasEnemyFlag = true;

                Point enemyFlagLocation = player.Team == Team.Blue ? GameSettings.RedFlagLocation : GameSettings.BlueFlagLocation;
                CaptureTheFlagBot.PlaceBlock(0, enemyFlagLocation.X, enemyFlagLocation.Y, 0);

                IncreaseGameFund(GameFundIncreaseReason.FlagTaken);

                CaptureTheFlagBot.SendChatMessage($"Player {player.Username} has taken the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag!");
            }
        }

        public static void CaptureFlag(Player player)
        {
            if (player.IsInGodMode || player.Team == Team.None)
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
                    BlueTeamScore = RedTeamScore = 0;

                    GameOver(player.Team);
                }
            }
        }

        public static void ReturnFlag(Player player, bool playerDied)
        {
            if (player.HasEnemyFlag)
            {
                player.HasEnemyFlag = false;

                if (player.Team == Team.Blue)
                {
                    RedFlagTaken = false;
                }
                else // Red team
                {
                    BlueFlagTaken = false;
                }

                Point enemyFlagLocation = player.Team == Team.Blue ? GameSettings.RedFlagLocation : GameSettings.BlueFlagLocation;
                int flagMorphId = player.Team == Team.Blue ? 4 : 1;
                CaptureTheFlagBot.PlaceBlock(0, enemyFlagLocation.X, enemyFlagLocation.Y, 327, flagMorphId);

                if (playerDied)
                {
                    StringBuilder teamName = new StringBuilder(TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team)));
                    teamName[0] = char.ToUpper(teamName[0]);

                    CaptureTheFlagBot.SendChatMessage($"Player {player.Username} died! {teamName} flag returned to base.");
                }
            }
        }

        public static void UpdateScore(Team team)
        {
            if (team == Team.None)
            {
                throw new InvalidEnumArgumentException();
            }

            if (team == Team.Blue)
            {
                BlueTeamScore++;
            }
            else // Red team
            {
                RedTeamScore++;
            }
        }

        public static void GameOver(Team winningTeam)
        {
            int coinsWon = GetSplitGameFund(winningTeam);

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

            GameFund = 0;

            CaptureTheFlagBot.LoadLevel();
            CaptureTheFlagBot.SendChatMessage($"Game over! Team {TeamHelper.EnumToString(winningTeam)} has won the game.");

            PlayersDatabaseTable.Save();
        }

        private static int GetSplitGameFund(Team winningTeam)
        {
            int totalTeamPlayers = 0;

            foreach (Player player in CaptureTheFlagBot.PlayersInWorld.Values)
            {
                if (player.Team == winningTeam)
                {
                    totalTeamPlayers++;
                }
            }

            return GameFund / totalTeamPlayers;
        }
    }
}