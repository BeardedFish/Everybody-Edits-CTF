// File Name:     GodModeToggledHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class GodModeToggledHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a player toggles God mode in the Everybody Edits world.
        /// </summary>
        public GodModeToggledHandler() : base(new string[] { EverybodyEditsMessage.GodModeToggled }, new IGameMechanic[] { new AntiCheat(), new SpectateMode() })
        {

        }

        /// <summary>
        /// Handles a player toggling God mode in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            int playerId = message.GetInt(0);
            bool isInGodMode = message.GetBoolean(1);

            if (ctfBot.JoinedWorld.Players.ContainsKey(playerId))
            {
                ctfBot.JoinedWorld.Players[playerId].IsInGodMode = isInGodMode;

                ExecuteGameMechanics(ctfBot, message.Type, ctfBot.JoinedWorld.Players[playerId]);
            }
        }
    }
}