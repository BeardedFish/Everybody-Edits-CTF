// File Name:     EffectHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class EffectHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a player receives/loses an effect in the Everybody Edits world.
        /// </summary>
        public EffectHandler() : base(new string[] { EverybodyEditsMessage.Effect }, new IGameMechanic[] { CtfGameRound.AntiCheat })
        {

        }

        /// <summary>
        /// Handles a player when they receive/lose an effect in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            int playerId = message.GetInt(0);

            if (ctfBot.JoinedWorld.Players.ContainsKey(playerId))
            {
                int effectId = message.GetInt(1);
                bool effectEnabled = message.GetBoolean(2);

                if (effectId != (int)Effect.Lava && effectEnabled)
                {
                    ExecuteGameMechanics(ctfBot, message.Type, ctfBot.JoinedWorld.Players[playerId]);

                    // Reset the flag variable
                    ctfBot.JoinedWorld.Players[playerId].PurchasedItemLegally = false;
                }
            }
        }
    }
}