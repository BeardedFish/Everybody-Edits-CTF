// File Name:     WorldActionHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Deserializer;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class WorldActionHandler : BotEvent
    {
        /// <summary>
        /// Event handler for when a world action (world cleared, world reloaded, and a system message) is received.
        /// </summary>
        public WorldActionHandler() : base(new string[] { EverybodyEditsMessage.ClearWorld, EverybodyEditsMessage.ReloadWorld, EverybodyEditsMessage.SystemMessage }, null)
        {

        }

        /// <summary>
        /// Handles a world action in the Everybody Edits world.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="message">The message to be handled. This message MUST match the one(s) defined in <see cref="BotEvent.TriggerMessages"/>. If not matched, runtime errors can appear.</param>
        public override void Handle(CtfBot ctfBot, Message message)
        {
            if (message.Type == EverybodyEditsMessage.ClearWorld)
            {
                if (ctfBot.JoinedWorld.Blocks != null)
                {
                    int blockId;

                    for (int layer = 0; layer < ctfBot.JoinedWorld.Blocks.GetLength(0); layer++)
                    {
                        for (int x = 0; x < ctfBot.JoinedWorld.Blocks.GetLength(1); x++)
                        {
                            for (int y = 0; y < ctfBot.JoinedWorld.Blocks.GetLength(2); y++)
                            {
                                if (x == 0 || y == 0 || x == ctfBot.JoinedWorld.Width - 1 || y == ctfBot.JoinedWorld.Height - 1) // Border block
                                {
                                    blockId = message.GetInt(2);
                                }
                                else // Fill block
                                {
                                    blockId = message.GetInt(3);
                                }

                                ctfBot.JoinedWorld.Blocks[layer, x, y] = new Block(blockId);
                            }
                        }
                    }
                }
            }
            else if (message.Type == EverybodyEditsMessage.ReloadWorld)
            {
                ctfBot.JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, ctfBot.JoinedWorld.Width, ctfBot.JoinedWorld.Height);
            }
            else // System messages
            {
                string[] messageWords = message.GetString(1).Split();

                if (messageWords[1] == "kicked")
                {
                    string kickedUsername = messageWords[2];
                    int playerId = ctfBot.JoinedWorld.GetPlayerId(kickedUsername);

                    if (playerId != -1)
                    {
                        ctfBot.JoinedWorld.Players.Remove(playerId);
                    }
                }
            }
        }
    }
}