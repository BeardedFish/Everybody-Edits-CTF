// File Name:     SmileyChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class SmileyChangedHandler : BotEvent
    {
        public SmileyChangedHandler() : base(new string[] { EverybodyEditsMessage.ClearWorld }, null)
        public SmileyChangedHandler() : base(new string[] { EverybodyEditsMessage.SmileyChanged }, null)
        {

        }

        public override void Handle(Message message)
        {
            int playerId = message.GetInt(0);
            int smileyId = message.GetInt(1);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                JoinedWorld.Players[playerId].SmileyId = smileyId;

                if (JoinedWorld.Players[playerId].IsPlayingGame && smileyId == (int)Smiley.Nurse)
                {
                    CtfBot.SendPrivateMessage(JoinedWorld.Players[playerId], "You are now a healer for your team!");
                }
            }
        }
    }
}