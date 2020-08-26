// File Name:     FlagSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class FlagSystem
    {
        /// <summary>
        /// States the max score a team needs to achieve in order to win the game.
        /// </summary>
        public const ushort MaxScoreToWin = 5;

        /// <summary>
        /// The locations of the blue/red team's flag in the world that the bot joined.
        /// </summary>
        public static readonly Point BlueFlagLocation = new Point(32, 101), RedFlagLocation = new Point(367, 101);

        /// <summary>
        /// A dictionary that contains the team flags for the Capture The Flag game. To access a specific teams flag, use the <see cref="Team"/> enum as the key.
        /// </summary>
        public Dictionary<Team, Flag> Flags = new Dictionary<Team, Flag>()
        {
            { Team.Blue, new Flag(Team.Blue, BlueFlagLocation, new MorphableBlock(327, 1)) },
            { Team.Red, new Flag(Team.Red, RedFlagLocation, new MorphableBlock(327, 4)) },
        };

        /// <summary>
        /// Game mechanic which handles the flag system in the Capture The Flag game. The flag system consists of capturing, returning, and taking a flag.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public FlagSystem(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnPlayerMoved += OnPlayerMoved;
        }

        /// <summary>
        /// Handles a player trying to capture, return, or take a flag in the Capture The Flag game. If the <see cref="MaxScoreToWin"/> is reached once a flag is captured,
        /// then the game is ended via the <see cref="CtfGameRound.End(CaptureTheFlagBot, Team)"/> method.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnPlayerMoved(CaptureTheFlagBot ctfBot, Player player)
        {
            if (player.IsInGodMode || !player.IsPlayingGame)
            {
                return;
            }

            Flag friendlyFlag = Flags[player.Team];
            Flag enemyFlag = Flags[player.Team.GetOppositeTeam()];

            if (enemyFlag.CanBeCapturedBy(player, friendlyFlag))
            {
                enemyFlag.Capture(ctfBot);

                ctfBot.CurrentGameRound.Scores[player.Team]++;
                ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.FlagCaptured);
                ctfBot.SayChatMessage(ctfBot.CurrentGameRound.GetScoresString());

                if (ctfBot.CurrentGameRound.Scores[player.Team] >= MaxScoreToWin)
                {
                    ctfBot.CurrentGameRound.End(ctfBot, player.Team);
                }
                else
                {
                    CelebrateCapture(ctfBot, player.Team);
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

        /// <summary>
        /// Places a firework above teams flag. This method should only be called when a player captures a flag. This method will throw an exception if the
        /// team is set to <see cref="Team.None"/>.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="team">The team that captured the flag.</param>
        private void CelebrateCapture(CaptureTheFlagBot ctfBot, Team team)
        {
            if (team == Team.None)
            {
                throw new Exception("Unsupported team type!");
            }

            Task.Run(async() =>
            {
                MorphableBlock firework = team == Team.Blue ? Blocks.Foreground.BlueFirework : Blocks.Foreground.RedFirework;
                Point placeLocation = (team == Team.Blue ? Flags[Team.Blue].HomeLocation : Flags[Team.Red].HomeLocation);

                await Task.Delay(500);

                placeLocation.Offset(0, -3);
                ctfBot.PlaceBlock(BlockLayer.Foreground, placeLocation, firework);

                await Task.Delay(1000);

                ctfBot.PlaceBlock(BlockLayer.Foreground, placeLocation, Blocks.None);
            });
        }
    }
}