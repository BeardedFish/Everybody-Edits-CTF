// File Name:     GodModeToggledHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class GodModeToggledHandler : BotEvent
    {
        public GodModeToggledHandler() : base(new string[] { EverybodyEditsMessage.GodModeToggled }, new IGameMechanic[] { new AntiCheat(), new SpectateMode() })
        {

        }

        public override void Handle(Message message)
        {
            base.Handle(message);

            int playerId = message.GetInt(0);
            bool isInGodMode = message.GetBoolean(1);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                JoinedWorld.Players[playerId].IsInGodMode = isInGodMode;

                ExecuteGameMechanics(message.Type, JoinedWorld.Players[playerId]);
            }
        }
    }
}