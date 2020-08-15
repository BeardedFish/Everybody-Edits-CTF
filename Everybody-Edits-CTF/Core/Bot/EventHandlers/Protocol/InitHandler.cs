// File Name:     InitHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
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
        /// Event handler for when the bot initially joins the world. This class modifies the properties defined in the <see cref="WorldInformation"/> class..
        /// </summary>
        public InitHandler() : base(new string[] { EverybodyEditsMessage.InitBegin, EverybodyEditsMessage.InitEnd }, null)
        {

        }

        /// <summary>
        /// Handles a bot initially joining an Everybody Edits world by getting information about it and doing a few events (such as moving).
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            if (message.Type == EverybodyEditsMessage.InitBegin)
            {
                ctfBot.JoinedWorld.Width = message.GetInt(18);
                ctfBot.JoinedWorld.Height = message.GetInt(19);
                ctfBot.JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, ctfBot.JoinedWorld.Width, ctfBot.JoinedWorld.Height);

                ctfBot.Send(EverybodyEditsMessage.InitEnd);
            }
            else // Init end
            {
                ctfBot.SetWorldTitle($"{BotSettings.WorldTitle} [ON]");
                ctfBot.SetGodMode(true);
                ctfBot.Move(BotSettings.JoinLocation);
                ctfBot.SendChatMessage("Connected!");

                Logger.WriteLog(LogType.EverybodyEditsMessage, "Connected to Everybody Edits succesfully!");
            }
        }
    }
}