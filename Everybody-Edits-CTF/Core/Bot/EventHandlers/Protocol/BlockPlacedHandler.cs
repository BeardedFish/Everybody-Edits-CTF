// File Name:     BlockPlacedHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using PlayerIOClient;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.Protocol
{
    public sealed class BlockPlacedHandler : EverybodyEditsBotEvent
    {
        public BlockPlacedHandler() : base(new string[]
            {
                EverybodyEditsMessage.MorphableBlockPlaced,
                EverybodyEditsMessage.NonPlayableCharacterBlockPlaced,
                EverybodyEditsMessage.MorphableBlockPlaced,
                EverybodyEditsMessage.PlaceBlock,
                EverybodyEditsMessage.SignBlockPlaced,
                EverybodyEditsMessage.WorldPortalBlockPlaced
            },
            null)
        {

        }

        public override void Handle(Message message)
        {
            base.Handle(message);

            if (JoinedWorld.Blocks != null)
            {
                switch (message.Type)
                {
                    case EverybodyEditsMessage.MorphableBlockPlaced:
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            int morphId = message.GetInt(3);
                            int layer = message.GetInt(4);

                            JoinedWorld.Blocks[layer, xLoc, yLoc] = new MorphableBlock(blockId, morphId);
                        }
                        break;
                    case EverybodyEditsMessage.MusicBlockPlaced:
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            int soundId = message.GetInt(3);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new MusicBlock(blockId, soundId);
                        }
                        break;
                    case EverybodyEditsMessage.NonPlayableCharacterBlockPlaced:
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            string name = message.GetString(3);
                            string[] messages = new string[] { message.GetString(4), message.GetString(5), message.GetString(6) };

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new NonPlayableCharacterBlock(blockId, name, messages);
                        }
                        break;
                    case EverybodyEditsMessage.PlaceBlock:
                        {
                            int layerId = message.GetInt(0);
                            uint xLoc = message.GetUInt(1);
                            uint yLoc = message.GetUInt(2);
                            int blockId = message.GetInt(3);

                            JoinedWorld.Blocks[layerId, xLoc, yLoc] = new Block(blockId);
                        }
                        break;
                    case EverybodyEditsMessage.SignBlockPlaced:
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            string text = message.GetString(3);
                            int signColour = message.GetInt(4);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new SignBlock(blockId, text, signColour);
                        }
                        break;
                    case EverybodyEditsMessage.WorldPortalBlockPlaced:
                        {
                            int xLoc = message.GetInt(0);
                            int yLoc = message.GetInt(1);
                            int blockId = message.GetInt(2);
                            string targetWorldId = message.GetString(3);
                            int targetSpawnId = message.GetInt(4);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new WorldPortalBlock(blockId, targetWorldId, targetSpawnId);
                        }
                        break;
                }
            }
        }
    }
}