// File Name:     WorldDeserializer.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Wednesday, July 15, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Deserializer.Blocks;
using PlayerIOClient;
using System;

namespace Everybody_Edits_CTF.Core.Deserializer
{
    public static class WorldDeserializer
    {
        /// <summary>
        /// Deserializes blocks from a Message object and stores the deserialized values in a 3 dimensional array of type Block. The first dimension is the layer, the second is the
        /// horizontal (x) positon, and the third is the vertical (y) posiition.
        /// </summary>
        /// <param name="m">The Message object that contains the data about the Everybody Edits world.</param>
        /// <param name="worldWidth">The width of the Everybody Edits world.</param>
        /// <param name="worldHeight">The height of the Everybody Edits world.</param>
        /// <returns></returns>
        public static Block[,,] DeserializeBlocks(Message m, int worldWidth, int worldHeight)
        {
            if (!CanDeserializeBlocks(m))
            {
                throw new Exception($"Can not deserialize world blocks from \"{m.Type}\" message.");
            }

            Block[,,] worldBlocks = InitalizeWorldBlocksArray(2, worldWidth, worldHeight);

            uint currentBlockChunk = GetWorldStart(m) + 1;
            while (m[currentBlockChunk] as string != EverybodyEditsMessage.WorldEnd)
            {
                int blockId = m.GetInt(currentBlockChunk);
                int layer = m.GetInt(currentBlockChunk + 1);
                byte[] xPositions = m.GetByteArray(currentBlockChunk + 2);
                byte[] yPositions = m.GetByteArray(currentBlockChunk + 3);
                uint chunkArgsRead = 4;

                for (int i = 0; i < xPositions.Length; i += 2)
                {
                    int x = (xPositions[i] << 8) | (xPositions[i + 1]);
                    int y = (yPositions[i] << 8) | (yPositions[i + 1]);

                    // TODO: Probably should move the number values to an enum for clarification...

                    switch (blockId)
                    {
                        case 77:
                        case 83:
                        case 1520:
                            {
                                int soundId = m.GetInt(currentBlockChunk + 4);
                                worldBlocks[layer, x, y] = new MusicBlock(blockId, soundId);

                                chunkArgsRead = 5;
                            }
                            break;
                        case 113:
                        case 184:
                        case 185:
                        case 467:
                        case 1619:
                        case 1620:
                        case 1079:
                        case 1080:
                            {
                                uint switchId = m.GetUInt(currentBlockChunk + 4);
                                worldBlocks[layer, x, y] = new SwitchBlock(blockId, switchId);

                                chunkArgsRead = 5;
                            }
                            break;
                        case 242:
                        case 381:
                            {
                                int portalId = m.GetInt(currentBlockChunk + 4);
                                int targetId = m.GetInt(currentBlockChunk + 5);
                                int rotationId = m.GetInt(currentBlockChunk + 6);
                                worldBlocks[layer, x, y] = new PortalBlock(blockId, portalId, targetId, rotationId);

                                chunkArgsRead = 7;

                            }
                            break;
                        case 1582:
                        {
                            int spawnId = m.GetInt(currentBlockChunk + 4);
                            worldBlocks[layer, x, y] = new WorldPortalSpawnBlock(blockId, spawnId);

                            chunkArgsRead = 5;
                        }
                        break;
                        case 374:
                            {
                                string targetWorldId = m.GetString(currentBlockChunk + 4);
                                int targetSpawn = m.GetInt(currentBlockChunk + 5);
                                worldBlocks[layer, x, y] = new WorldPortalBlock(blockId, targetWorldId, targetSpawn);

                                chunkArgsRead = 6;
                            }
                            break;
                        case 385:
                            {
                                string text = m.GetString(currentBlockChunk + 4);
                                int rotationId = m.GetInt(currentBlockChunk + 5);
                                worldBlocks[layer, x, y] = new SignBlock(blockId, text, rotationId);

                                chunkArgsRead = 6;
                            }
                            break;
                        case 417:
                        case 418:
                        case 419:
                        case 420:
                        case 423:
                        case 453:
                        case 461:
                        case 1517:
                        case 1584:
                            {
                                int effectId = m.GetInt(currentBlockChunk + 4);
                                worldBlocks[layer, x, y] = new EffectBlock(blockId, effectId);

                                chunkArgsRead = 5;
                            }
                            break;
                        case 421:
                        case 422:
                            {
                                int duration = m.GetInt(currentBlockChunk + 4);
                                worldBlocks[layer, x, y] = new TimedEffectBlock(blockId, duration);

                                chunkArgsRead = 5;
                            }
                            break;
                        case 1550:
                        case 1551:
                        case 1552:
                        case 1553:
                        case 1554:
                        case 1556:
                        case 1557:
                        case 1558:
                        case 1571:
                        case 1572:
                        case 1573:
                        case 1576:
                        case 1579:
                            {
                                string name = m.GetString(currentBlockChunk + 4);
                                string[] messages = { m.GetString(currentBlockChunk + 5), m.GetString(currentBlockChunk + 6), m.GetString(currentBlockChunk + 7) };
                                worldBlocks[layer, x, y] = new NonPlayableCharacterBlock(blockId, name, messages);

                                chunkArgsRead = 8;
                            }
                            break;
                        case 273:
                        case 275:
                        case 276:
                        case 277:
                        case 279:
                        case 280:
                        case 327:
                        case 328:
                        case 329:
                        case 338:
                        case 339:
                        case 340:
                        case 361:
                        case 376:
                        case 377:
                        case 378:
                        case 379:
                        case 380:
                        case 438:
                        case 439:
                        case 440:
                        case 447:
                        case 448:
                        case 449:
                        case 451:
                        case 452:
                        case 456:
                        case 457:
                        case 458:
                        case 464:
                        case 465:
                        case 471:
                        case 499:
                        case 1001:
                        case 1002:
                        case 1003:
                        case 1004:
                        case 1041:
                        case 1042:
                        case 1043:
                        case 1052:
                        case 1053:
                        case 1054:
                        case 1055:
                        case 1056:
                        case 1092:
                        case 1134:
                        case 1135:
                        case 1140:
                        case 1141:
                        case 1500:
                        case 1502:
                        case 1506:
                        case 1507:
                        case 1535:
                        case 1536:
                        case 1537:
                        case 1538:
                        case 1155:
                        case 1160:
                        case 1581:
                        case 1587:
                        case 1588:
                        case 1592:
                        case 1593:
                        case 1594:
                        case 1595:
                        case 1596:
                        case 1597:
                        case 1605:
                        case 1606:
                        case 1607:
                        case 1608:
                        case 1609:
                        case 1610:
                        case 1611:
                        case 1612:
                        case 1613:
                        case 1614:
                        case 1615:
                        case 1616:
                        case 1617:
                        case 1633:
                            {
                                int morphId = m.GetInt(currentBlockChunk + 4);
                                worldBlocks[layer, x, y] = new MorphableBlock(blockId, morphId);

                                chunkArgsRead = 5;
                            }
                            break;
                        default:
                            {
                                worldBlocks[layer, x, y] = new Block(blockId);

                                chunkArgsRead = 4;
                            }
                            break;
                    }
                }

                currentBlockChunk += chunkArgsRead;
            }

            return worldBlocks;
        }

        /// <summary>
        /// States whether a Message object can be deserialized for world block data or not.
        /// </summary>
        /// <param name="m">The Message object to be checked.</param>
        /// <returns>True if the Message object can be deserialized, if not , false.</returns>
        private static bool CanDeserializeBlocks(Message m)
        {
            return m.Type == EverybodyEditsMessage.InitBegin || m.Type == EverybodyEditsMessage.ReloadWorld;
        }

        /// <summary>
        /// Finds the index where <see cref="EverybodyEditsMessage.WorldStart"/> is located in the arguments of the Message object. This method should only be called if the Message
        /// object does indeed have the <see cref="EverybodyEditsMessage.WorldStart"/> message. To ensure it does, use the <see cref="CanDeserializeBlocks"/> method.
        /// </summary>
        /// <param name="m">The Message object to be searched for <see cref="EverybodyEditsMessage.WorldStart"/>.</param>
        /// <returns>A uint which represents the location where the <see cref="EverybodyEditsMessage.WorldStart"/> message is located.</returns>
        private static uint GetWorldStart(Message m)
        {
            uint worldStartIndex = 0;
            while (m[worldStartIndex] as string != EverybodyEditsMessage.WorldStart)
            {
                worldStartIndex++;
            }

            return worldStartIndex;
        }

        /// <summary>
        /// Creates and initializes a 3 dimensional array of type <see cref="Block"/>.
        /// </summary>
        /// <param name="totalLayers">The total number of layers to create for the array.</param>
        /// <param name="worldWidth">The total width of the array, in blocks.</param>
        /// <param name="worldHeight">The total height of the array, in blocks.</param>
        /// <returns>A 3 dimensional array of type <see cref="Block"/> which contains empty blocks.</returns>
        private static Block[,,] InitalizeWorldBlocksArray(int totalLayers, int worldWidth, int worldHeight)
        {
            Block[,,] worldBlocks = new Block[totalLayers, worldWidth, worldHeight];

            for (int layer = 0; layer < totalLayers; layer++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    for (int y = 0; y < worldHeight; y++)
                    {
                        worldBlocks[layer, x, y] = new Block();
                    }
                }
            }

            return worldBlocks;
        }
    }
}