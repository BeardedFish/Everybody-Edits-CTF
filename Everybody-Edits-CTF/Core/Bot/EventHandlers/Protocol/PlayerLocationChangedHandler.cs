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
        public PlayerLocationChangedHandler() : base(new string[] { EverybodyEditsMessage.PlayerMoved, EverybodyEditsMessage.PlayerTeleported }, new IGameMechanic[] { new FightSystem(), CtfGameRound.FlagSystem, new HealSystem(), new RoomEntrance(), new Shop(), new TrapSystem(), new WarpPipe() })
        {

        }

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