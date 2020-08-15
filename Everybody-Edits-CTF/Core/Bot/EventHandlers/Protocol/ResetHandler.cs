// File Name:     ResetHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Helpers;
using PlayerIOClient;
using System;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class ResetHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a player is reset in the Everybody Edits world.
        /// </summary>
        public ResetHandler() : base(new string[] { EverybodyEditsMessage.PlayerReset }, null)
        {

        }

        /// <summary>
        /// Handles a player being reset in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            if (message.Count >= 6)
            {
                bool propertiesReset = message.GetBoolean(0);

                for (uint i = 2; i <= message.Count - 4; i += 4)
                {
                    int playerId = message.GetInt(i);

                    if (ctfBot.JoinedWorld.Players.ContainsKey(playerId))
                    {
                        double xLoc = Math.Round(message.GetDouble(i + 1) / 16.0);
                        double yLoc = Math.Round(message.GetDouble(i + 2) / 16.0);
                        int deathCount = message.GetInt(i + 3);

                        ctfBot.JoinedWorld.Players[playerId].UpdateLocation((int)xLoc, (int)yLoc);

                        if (propertiesReset)
                        {
                            ctfBot.JoinedWorld.Players[playerId].Team = Team.None;
                        }
                        else
                        {
                            if (ctfBot.JoinedWorld.Players[playerId].LastAttacker != null)
                            {
                                ctfBot.SendPrivateMessage(ctfBot.JoinedWorld.Players[playerId], $"You were killed by player {ctfBot.JoinedWorld.Players[playerId].LastAttacker.Username}!");
                                ctfBot.SendPrivateMessage(ctfBot.JoinedWorld.Players[playerId].LastAttacker, $"You killed player {ctfBot.JoinedWorld.Players[playerId].Username}!");

                                if (PlayersDatabaseTable.Loaded)
                                {
                                    PlayerDatabaseRow playerData = PlayersDatabaseTable.GetRow(ctfBot.JoinedWorld.Players[playerId].LastAttacker.Username);

                                    if (playerData != null)
                                    {
                                        playerData.TotalKills++;
                                    }
                                }

                                ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.PlayerKilledEnemy);
                            }

                            ctfBot.JoinedWorld.Players[playerId].LastAttacker = null;
                        }

                        if (deathCount > ctfBot.JoinedWorld.Players[playerId].DeathCount)
                        {
                            ctfBot.JoinedWorld.Players[playerId].Respawn(ctfBot);

                            Team enemyTeam = TeamHelper.GetOppositeTeam(ctfBot.JoinedWorld.Players[playerId].Team);
                            if (ctfBot.CurrentGameRound.FlagSystem.Flags[enemyTeam].Holder == ctfBot.JoinedWorld.Players[playerId])
                            {
                                ctfBot.SendChatMessage($"Player {ctfBot.JoinedWorld.Players[playerId].Username} died while holding {TeamHelper.EnumToString(enemyTeam)} teams flag.");

                                ctfBot.CurrentGameRound.FlagSystem.Flags[enemyTeam].Return(ctfBot, null, false);
                            }

                            ctfBot.RemoveEffects(ctfBot.JoinedWorld.Players[playerId]);
                        }

                        ctfBot.JoinedWorld.Players[playerId].DeathCount = deathCount;
                    }
                }
            }
        }
    }
}