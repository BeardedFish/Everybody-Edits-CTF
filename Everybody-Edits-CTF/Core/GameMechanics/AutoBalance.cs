﻿// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 20, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class AutoBalance
    {
        public static void Handle(Player player, Dictionary<int, Player> playersInWorld)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }
            
            string resultMsg = $"You joined the {TeamHelper.EnumToString(player.Team)} team!";
            int joinedTeamTotalPlayers = TeamHelper.TotalPlayers(playersInWorld, player.Team) - 1;
            int oppositeTeamTotalPlayers = TeamHelper.TotalPlayers(playersInWorld, TeamHelper.GetOppositeTeam(player.Team));

            if (GameSettings.AutoBalanceTeams)
            {
                if (joinedTeamTotalPlayers > oppositeTeamTotalPlayers)
                {
                    resultMsg = "Unbalanced teams! You have been transferred to the other team!";

                    Point teleLocation = player.Team == Team.Blue ? GameSettings.RedSpawnLocation : GameSettings.BlueSpawnLocation;
                    CaptureTheFlagBot.TeleportPlayer(player, teleLocation.X, teleLocation.Y);
                }
            }

            CaptureTheFlagBot.SendPrivateMessage(player, resultMsg);
        }
    }
}