// File Name:     PlayerTeamChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class PlayerTeamChangedHandler : EverybodyEditsBotEvent
    {
        public PlayerTeamChangedHandler() : base(new string[] { EverybodyEditsMessage.TeamChanged }, new IGameMechanic[] { new AutoBalance() })
        {

        }

        public override void Handle(Message message)
        {
            base.Handle(message);

            int playerId = message.GetInt(0);
            int teamId = message.GetInt(1);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                JoinedWorld.Players[playerId].Team = (Team)teamId;

                ExecuteGameMechanics(message.Type, JoinedWorld.Players[playerId]);
            }
        }
    }
}