// File Name:     LeftWorldHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Helpers;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class LeftWorldHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a player leaves the Everybody Edits world.
        /// </summary>
        public LeftWorldHandler() : base(new string[] { EverybodyEditsMessage.PlayerLeftWorld, EverybodyEditsMessage.PlayerJoinedWorld }, null)
        {

        }

        /// <summary>
        /// Handles a player leaving the Everybody Edits world.
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
            int playerId = message.GetInt(0);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                string username = JoinedWorld.Players[playerId].Username;

                if (JoinedWorld.Players[playerId].HasEnemyFlag)
                {
                    CtfGameRound.FlagSystem.Flags[TeamHelper.GetOppositeTeam(JoinedWorld.Players[playerId].Team)].Return(null, false);
                }

                JoinedWorld.Players.Remove(playerId);

                Logger.WriteLog(LogType.EverybodyEditsMessage, $"Player {username} (id: {playerId}) has left the world.");
            }
        }
    }
}