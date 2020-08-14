// File Name:     PlayerResetHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Helpers;
using PlayerIOClient;
using System;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class PlayerResetHandler : BotEvent
    {
        public PlayerResetHandler() : base(new string[] { EverybodyEditsMessage.PlayerReset }, null)
        {

        }

        public override void Handle(Message message)
        {
            if (message.Count >= 6)
            {
                bool propertiesReset = message.GetBoolean(0);

                for (uint i = 2; i <= message.Count - 4; i += 4)
                {
                    int playerId = message.GetInt(i);

                    if (JoinedWorld.Players.ContainsKey(playerId))
                    {
                        double xLoc = Math.Round(message.GetDouble(i + 1) / 16.0);
                        double yLoc = Math.Round(message.GetDouble(i + 2) / 16.0);
                        int deathCount = message.GetInt(i + 3);

                        JoinedWorld.Players[playerId].UpdateLocation((int)xLoc, (int)yLoc);

                        if (propertiesReset)
                        {
                            JoinedWorld.Players[playerId].Team = Team.None;
                        }
                        else
                        {
                            if (JoinedWorld.Players[playerId].LastAttacker != null)
                            {
                                CtfBot.SendPrivateMessage(JoinedWorld.Players[playerId], $"You were killed by player {JoinedWorld.Players[playerId].LastAttacker.Username}!");
                                CtfBot.SendPrivateMessage(JoinedWorld.Players[playerId].LastAttacker, $"You killed player {JoinedWorld.Players[playerId].Username}!");

                                if (PlayersDatabaseTable.Loaded)
                                {
                                    PlayerDatabaseRow playerData = PlayersDatabaseTable.GetRow(JoinedWorld.Players[playerId].LastAttacker.Username);

                                    if (playerData != null)
                                    {
                                        playerData.TotalKills++;
                                    }
                                }

                                // GameFund.Increase(GameFundIncreaseReason.PlayerKilledEnemy);
                            }

                            JoinedWorld.Players[playerId].LastAttacker = null;
                        }

                        if (deathCount > JoinedWorld.Players[playerId].DeathCount)
                        {
                            JoinedWorld.Players[playerId].Respawn();

                            Team enemyTeam = TeamHelper.GetOppositeTeam(JoinedWorld.Players[playerId].Team);
                            if (CtfGameRound.FlagSystem.Flags[enemyTeam].Holder == JoinedWorld.Players[playerId])
                            {
                                CtfBot.SendChatMessage($"Player {JoinedWorld.Players[playerId].Username} died while holding {TeamHelper.EnumToString(enemyTeam)} teams flag.");

                                CtfGameRound.FlagSystem.Flags[enemyTeam].Return(null, false);
                            }

                            CtfBot.RemoveEffects(JoinedWorld.Players[playerId]);
                        }

                        JoinedWorld.Players[playerId].DeathCount = deathCount;
                    }
                }
            }
        }
    }
}