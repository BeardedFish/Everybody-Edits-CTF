// File Name:     PlayerLocationChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class PlayerLocationChangedHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a players location is changed in the Everybody Edits world.
        /// </summary>
        public PlayerLocationChangedHandler() : base(new string[] { EverybodyEditsMessage.PlayerMoved, EverybodyEditsMessage.PlayerTeleported }, new IGameMechanic[] { new FightSystem(), CtfGameRound.FlagSystem, new HealSystem(), new RoomEntrance(), new Shop(), new TrapSystem(), new WarpPipe() })
        {

        }

        /// <summary>
        /// Handles a players location changing in the Everybody Edits world.
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
            int playerId = message.GetInt(0);

            if (JoinedWorld.Players.ContainsKey(playerId) && JoinedWorld.Players[playerId].IsPlayingGame)
            {
                JoinedWorld.Players[playerId].UpdateMovementInformation(message);

                ExecuteGameMechanics(message.Type, JoinedWorld.Players[playerId]);
            }
        }
    }
}