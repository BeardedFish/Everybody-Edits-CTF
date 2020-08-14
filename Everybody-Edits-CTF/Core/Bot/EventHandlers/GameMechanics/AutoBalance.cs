// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class AutoBalance : IGameMechanic
    {
        public void Handle(string messageType, Player player)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }

            string resultMsg = $"You joined the {TeamHelper.EnumToString(player.Team)} team!";
            int joinedTeamTotalPlayers = TeamHelper.TotalPlayers(JoinedWorld.Players, player.Team) - 1;
            int oppositeTeamTotalPlayers = TeamHelper.TotalPlayers(JoinedWorld.Players, TeamHelper.GetOppositeTeam(player.Team));

            if (GameSettings.AutoBalanceTeams)
            {
                if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
                {
                    resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                    Point teleLocation = player.Team == Team.Blue ? GameSettings.RedCheckpointLocation : GameSettings.BlueCheckpointLocation;
                    CtfBot.TeleportPlayer(player, teleLocation.X, teleLocation.Y);
                }
            }

            CtfBot.SendPrivateMessage(player, resultMsg);
        }
    }
}