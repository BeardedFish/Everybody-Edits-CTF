// File Name:     PlayerLeftWorldHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Helpers;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class PlayerLeftWorldHandler : BotEvent
    {
        public PlayerLeftWorldHandler() : base(new string[] { EverybodyEditsMessage.PlayerLeftWorld, EverybodyEditsMessage.PlayerJoinedWorld }, null)
        {

        }

        public override void Handle(Message message)
        {
            base.Handle(message);

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