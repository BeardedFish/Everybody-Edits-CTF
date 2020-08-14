// File Name:     PlayerTeamChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class PlayerTeamChangedHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a player joins/leaves a team in the Everybody Edits world.
        /// </summary>
        public PlayerTeamChangedHandler() : base(new string[] { EverybodyEditsMessage.TeamChanged }, new IGameMechanic[] { new AutoBalance() })
        {

        }

        /// <summary>
        /// Handles a player joining/leaving a team in the Everybody Edits world.
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
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