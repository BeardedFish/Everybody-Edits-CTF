// File Name:     InitHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Deserializer;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class InitHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when the bot initially joins the world. This class modifies the properties defined in the <see cref="JoinedWorld"/> class..
        /// </summary>
        public InitHandler() : base(new string[] { EverybodyEditsMessage.InitBegin, EverybodyEditsMessage.InitEnd }, null)
        {

        }

        /// <summary>
        /// Handles a bot initially joining an Everybody Edits world by getting information about it and doing a few events (such as moving).
        /// </summary>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(Message message)
        {
            if (message.Type == EverybodyEditsMessage.InitBegin)
            {
                JoinedWorld.Width = message.GetInt(18);
                JoinedWorld.Height = message.GetInt(19);
                JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, JoinedWorld.Width, JoinedWorld.Height);

                CtfBot.Send(EverybodyEditsMessage.InitEnd);
            }
            else // Init end
            {
                CtfBot.SetWorldTitle($"{BotSettings.WorldTitle} [ON]");
                CtfBot.SetGodMode(true);
                CtfBot.Move(BotSettings.JoinLocation);
                CtfBot.SendChatMessage("Connected!");

                Logger.WriteLog(LogType.EverybodyEditsMessage, "Connected to Everybody Edits succesfully!");
            }
        }
    }
}