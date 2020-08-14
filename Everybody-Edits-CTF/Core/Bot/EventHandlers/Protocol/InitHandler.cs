// File Name:     InitHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Deserializer;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class InitHandler : BotEvent
    {
        public InitHandler() : base(new string[] { EverybodyEditsMessage.InitBegin, EverybodyEditsMessage.InitEnd }, null)
        {

        }

        public override void Handle(Message message)
        {
            base.Handle(message);

            if (message.Type == EverybodyEditsMessage.InitBegin)
            {
                JoinedWorld.Width = message.GetInt(18);
                JoinedWorld.Height = message.GetInt(19);
                JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, JoinedWorld.Width, JoinedWorld.Height);

                CtfBot.Send(EverybodyEditsMessage.InitEnd);
            }
            else
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