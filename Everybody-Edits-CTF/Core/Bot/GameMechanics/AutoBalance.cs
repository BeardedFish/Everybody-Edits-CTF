// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.Enums.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class AutoBalance
    {
        /// <summary>
        /// Game mechanic which auto balances the teams in the Capture The Flag game.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public AutoBalance(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnTeamChanged += OnTeamChanged;
        }

        /// <summary>
        /// Handles team balance in the Capture The Flag game. If a player joins a team with more players than the other team, then they are transferred to the other team
        /// with less players.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnTeamChanged(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }

            string resultMsg = $"You joined the {player.Team.GetStringName()} team!";
            int joinedTeamTotalPlayers = CountTeamPlayers(ctfBot.JoinedWorld.Players, player.Team) - 1;
            int oppositeTeamTotalPlayers = CountTeamPlayers(ctfBot.JoinedWorld.Players, player.Team.GetOppositeTeam());

            if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
            {
                resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                Point teleLocation = player.Team == Team.Blue ? RespawnSystem.RedCheckpointLocation : RespawnSystem.BlueCheckpointLocation;
                ctfBot.TeleportPlayer(player, teleLocation.X, teleLocation.Y);
            }

            ctfBot.SendPrivateMessage(player, resultMsg);
        }

        /// <summary>
        /// Gets the total number of players on a specific team.
        /// </summary>
        /// <param name="playersInWorld">The dictionary of players in the Everybody Edits world.</param>
        /// <param name="team">The team a player must belong to in order to be accumulated.</param>
        /// <returns>An <see cref="int"/> that represents the number of players that are on the specified team.</returns>
        private int CountTeamPlayers(Dictionary<int, Player> playersInWorld, Team team)
        {
            return playersInWorld.Where(keyValuePair => keyValuePair.Value.Team == team).Count();
        }
    }
}