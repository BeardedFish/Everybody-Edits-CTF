// File Name:     WorldActionHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
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
                if (JoinedWorld.Blocks != null)
                {
                    int blockId;

                    for (int layer = 0; layer < JoinedWorld.Blocks.GetLength(0); layer++)
                    {
                        for (int x = 0; x < JoinedWorld.Blocks.GetLength(1); x++)
                        {
                            for (int y = 0; y < JoinedWorld.Blocks.GetLength(2); y++)
                            {
                                if (x == 0 || y == 0 || x == JoinedWorld.Width - 1 || y == JoinedWorld.Height - 1) // Border block
                                {
                                    blockId = message.GetInt(2);
                                }
                                else // Fill block
                                {
                                    blockId = message.GetInt(3);
                                }

                                JoinedWorld.Blocks[layer, x, y] = new Block(blockId);
                            }
                        }
                    }
                }
            }
            else if (message.Type == EverybodyEditsMessage.ReloadWorld)
            {
                JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(message, JoinedWorld.Width, JoinedWorld.Height);
            }
            else // System messages
            {
                string[] messageWords = message.GetString(1).Split();

                if (messageWords[1] == "kicked")
                {
                    string kickedUsername = messageWords[2];
                    int playerId = JoinedWorld.GetPlayerId(kickedUsername);

                    if (playerId != -1)
                    {
                        JoinedWorld.Players.Remove(playerId);
                    }
                }
            }
        }
    }
}