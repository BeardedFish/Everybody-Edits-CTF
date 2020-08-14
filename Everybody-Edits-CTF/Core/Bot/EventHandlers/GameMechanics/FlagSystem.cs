// File Name:     FlagSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class FlagSystem : IGameMechanic
    {
        /// <summary>
        /// A dictionary that contains the team flags for the Capture The Flag game. To access a specific teams flag, use the <see cref="Team"/> enum as the key.
        /// </summary>
        public Dictionary<Team, Flag> Flags = new Dictionary<Team, Flag>()
        {
            { Team.Blue, new Flag(Team.Blue, GameSettings.BlueFlagLocation, new MorphableBlock(327, 1)) },
            { Team.Red, new Flag(Team.Red, GameSettings.RedFlagLocation, new MorphableBlock(327, 4)) },
        };

        public void Handle(string messageType, Player player)
        {
            if (player.IsInGodMode || !player.IsPlayingGame)
            {
                return;
            }

            Flag friendlyFlag = Flags[player.Team];
            Flag enemyFlag = Flags[TeamHelper.GetOppositeTeam(player.Team)];

            if (enemyFlag.CanBeCapturedBy(player, friendlyFlag))
            {
                enemyFlag.Capture();

                CtfBot.CurrentGameRound.Scores[player.Team]++;
                CtfBot.SendChatMessage(CtfBot.CurrentGameRound.GetScoresString());
                CtfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.FlagCaptured);

                if (CtfBot.CurrentGameRound.Scores[player.Team] >= GameSettings.MaxScoreToWin)
                {
                    CtfBot.CurrentGameRound.End(player.Team);
                }
            }
            else if (enemyFlag.CanBeTakenBy(player))
            {
                enemyFlag.Take(player);

                CtfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.FlagTaken);
            }
            else if (friendlyFlag.CanBeReturnedBy(player))
            {
                friendlyFlag.Return(player, false);
            }
        }
    }
}