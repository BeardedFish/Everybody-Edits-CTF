﻿// File Name:     dJoinedWorldHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Helpers;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;
using System;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class JoinedWorldHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a player joins the Everybody Edits world. This class does not handle the bot joining the world. Refer to <see cref="InitHandler"/> for
        /// bot join handling code.
        /// </summary>
        public JoinedWorldHandler() : base(new string[] { EverybodyEditsMessage.PlayerJoinedWorld }, new IGameMechanic[] { new DailyBonus() } )
        {

        }

        /// <summary>
        /// Handles a player joining the Everybody Edits world.
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
            int playerId = message.GetInt(0);

            if (!JoinedWorld.Players.ContainsKey(playerId))
            {
                string username = message.GetString(1);
                int smileyId = message.GetInt(3);
                double xLoc = Math.Round(message.GetDouble(4) / 16.0);
                double yLoc = Math.Round(message.GetDouble(5) / 16.0);
                bool isInGodMode = message.GetBoolean(6);
                int teamId = message.GetInt(15);
                bool canToggleGodMode = message.GetBoolean(23);

                JoinedWorld.Players.Add(playerId, new Player(username,
                                                            smileyId,
                                                            new Point((int)xLoc, (int)yLoc),
                                                            isInGodMode,
                                                            TeamHelper.IdToEnum(teamId),
                                                            canToggleGodMode));

                if (PlayersDatabaseTable.Loaded)
                {
                    if (!JoinedWorld.Players[playerId].IsGuest)
                    {
                        if (!PlayersDatabaseTable.PlayerExists(username))
                        {
                            PlayersDatabaseTable.AddNewPlayer(username);

                            CtfBot.SendPrivateMessage(JoinedWorld.Players[playerId], "Welcome newcomer! Type !help to learn how to play in this world.");
                        }
                    }
                }

                ExecuteGameMechanics(message.Type, JoinedWorld.Players[playerId]);

                Logger.WriteLog(LogType.EverybodyEditsMessage, $"Player {username.ToUpper()} (id: {playerId}) has joined the world.");
            }
        }
    }
}