// File Name:     EditRightsChangedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class EditRightsChangedHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a when a player gains/loses their edit rights in the Everybody Edits world. This class modified the <see cref="JoinedWorld.Players"/>
        /// objects.
        /// </summary>
        public EditRightsChangedHandler() : base(new string[] { EverybodyEditsMessage.EditRightsChanged }, null)
        {

        }

        /// <summary>
        /// Handles when a player gains/loses their edit rights in the Everybody Edits world.
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
            int playerId = message.GetInt(0);

            if (JoinedWorld.Players.ContainsKey(playerId))
            {
                bool canEdit = message.GetBoolean(1);

                JoinedWorld.Players[playerId].CanToggleGodMode = canEdit; // A player that can edit will ALWAYS have access to God mode
            }
        }
    }
}