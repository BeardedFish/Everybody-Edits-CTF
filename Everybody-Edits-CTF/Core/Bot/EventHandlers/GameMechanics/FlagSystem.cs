// File Name:     FlagSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
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

        /// <summary>
        /// Handles the flag system in the Capture The Flag game. The flag system consists of capturing, returning, and taking a flag. If the <see cref="GameSettings.MaxScoreToWin"/>
        /// is reached after a flag is captured, then the game is ended via the <see cref="CtfGameRound.End(Team)"/> method.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(CtfBot ctfBot, string messageType, Player player)
        {
            if (player.IsInGodMode || !player.IsPlayingGame)
            {
                return;
            }

            Flag friendlyFlag = Flags[player.Team];
            Flag enemyFlag = Flags[TeamHelper.GetOppositeTeam(player.Team)];

            if (enemyFlag.CanBeCapturedBy(player, friendlyFlag))
            {
                enemyFlag.Capture(ctfBot);

                ctfBot.CurrentGameRound.Scores[player.Team]++;
                ctfBot.SendChatMessage(ctfBot.CurrentGameRound.GetScoresString());
                ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.FlagCaptured);

                if (ctfBot.CurrentGameRound.Scores[player.Team] >= GameSettings.MaxScoreToWin)
                {
                    ctfBot.CurrentGameRound.End(ctfBot, player.Team);
                }
            }
            else if (enemyFlag.CanBeTakenBy(player))
            {
                enemyFlag.Take(ctfBot, player);

                ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.FlagTaken);
            }
            else if (friendlyFlag.CanBeReturnedBy(player))
            {
                friendlyFlag.Return(ctfBot, player, false);
            }
        }
    }
}