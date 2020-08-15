// File Name:     LocationChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class LocationChangedHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a players location is changed in the Everybody Edits world.
        /// </summary>
        public LocationChangedHandler(FlagSystem flagSystem) : base(new string[] { EverybodyEditsMessage.PlayerMoved, EverybodyEditsMessage.PlayerTeleported }, new IGameMechanic[] { new FightSystem(), flagSystem, new HealSystem(), new RoomEntrance(), new Shop(), new TrapSystem(), new WarpPipe() })
        {

        }

        /// <summary>
        /// Handles a players location changing in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            int playerId = message.GetInt(0);

            if (ctfBot.JoinedWorld.Players.ContainsKey(playerId) && ctfBot.JoinedWorld.Players[playerId].IsPlayingGame)
            {
                ctfBot.JoinedWorld.Players[playerId].UpdateMovementInformation(message);

                ExecuteGameMechanics(ctfBot, message.Type, ctfBot.JoinedWorld.Players[playerId]);
            }
        }
    }
}