// File Name:     EditRightsChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class EditRightsChangedHandler : EverybodyEditsBotEvent
    {
        public EditRightsChangedHandler() : base(new string[] { EverybodyEditsMessage.EditRightsChanged }, null)
        {

        }

        public override void Handle(Message message)
        {
            base.Handle(message);

            int playerId = message.GetInt(0);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                bool canEdit = message.GetBoolean(1);

                JoinedWorld.Players[playerId].CanToggleGodMode = canEdit; // A player that can edit will ALWAYS have access to God mode
            }
        }
    }
}