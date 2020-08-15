// File Name:     SmileyChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class SmileyChangedHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a players smiley is changed.
        /// </summary>
        public SmileyChangedHandler() : base(new string[] { EverybodyEditsMessage.SmileyChanged }, null)
        {

        }

        /// <summary>
        /// Handles a player changing their smiley in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            int playerId = message.GetInt(0);
            int smileyId = message.GetInt(1);

            if (ctfBot.JoinedWorld.Players.ContainsKey(playerId))
            {
                ctfBot.JoinedWorld.Players[playerId].SmileyId = smileyId;

                if (ctfBot.JoinedWorld.Players[playerId].IsPlayingGame && smileyId == (int)Smiley.Nurse)
                {
                    ctfBot.SendPrivateMessage(ctfBot.JoinedWorld.Players[playerId], "You are now a healer for your team!");
                }
            }
        }
    }
}