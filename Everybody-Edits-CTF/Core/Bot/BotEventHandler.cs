// File Name:     BotEventHandler.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Core.Deserializer;
using Everybody_Edits_CTF.Core.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.GameMechanics;
using Everybody_Edits_CTF.Core.GameMechanics.Enums;
using Everybody_Edits_CTF.Core.GameMechanics.Traps;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using Everybody_Edits_CTF.Logging;
using PlayerIOClient;
using System;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot
{
    public class BotEventHandler
    {
        /// <summary>
        /// The traps that can be triggered by players playing the Capture the Flag game.
        /// </summary>
        private readonly Trap[] traps = new Trap[]
        {
            new BlueBaseTrap(),
            new BridgeTrap(),
            new LavaLakeTrap(),
            new RedBaseTrap()
        };

        /// <summary>
        /// Handles all messages that a Capture the Flag bot receives.
        /// </summary>
        /// <param name="connection">The connection of the bot to Everybody Edits.</param>
        public BotEventHandler(Connection connection)
        {
            connection.OnDisconnect += OnDisconnect;
            connection.OnMessage += OnMessage;
        }

        /// <summary>
        /// Event handler for when the bot is disconnected from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="message">The reason why the bot was disconnected.</param>
        private void OnDisconnect(object sender, string message)
        {
            JoinedWorld.Players.Clear();

            //CaptureTheFlag.ResetGameStatistics();
            PlayersDatabaseTable.Save();
            Logger.WriteLog(LogType.EverybodyEditsMessage, $"Disconnected from the Everybody Edits world (Reason: {message}).");
            
            // Only reconnect if the bot was not disconnected on purpose
            if (BotSettings.AutoReconnectOnDisconnect && message != "Disconnect")
            {
                Logger.WriteLog(LogType.EverybodyEditsMessage, "Auto reconnecting...");

                CaptureTheFlagBot.Connect();
            }
        }

        /// <summary>
        /// Event handler for every time the bot receives a message from Everybody Edits.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="m">The message object that contains data about the message received.</param>
        private void OnMessage(object sender, Message m)
        {
            switch (m.Type)
            {
                case EverybodyEditsMessage.ClearWorld:
                    {
                        int borderBlockId = m.GetInt(2);
                        int fillBlockId = m.GetInt(3);

                        if (JoinedWorld.Blocks != null)
                        {
                            for (int layer = 0; layer < JoinedWorld.Blocks.GetLength(0); layer++)
                            {
                                for (int x = 0; x < JoinedWorld.Blocks.GetLength(0); x++)
                                {
                                    for (int y = 0; y < JoinedWorld.Blocks.GetLength(0); y++)
                                    {
                                        JoinedWorld.Blocks[layer, x, y] = new Block(fillBlockId);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.InitBegin:
                    {
                        JoinedWorld.Width = m.GetInt(18);
                        JoinedWorld.Height = m.GetInt(19);
                        JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(m, JoinedWorld.Width, JoinedWorld.Height);

                        CaptureTheFlagBot.Send(EverybodyEditsMessage.InitEnd);
                    }
                    break;
                case EverybodyEditsMessage.InitEnd:
                    {
                        CaptureTheFlagBot.SetWorldTitle($"{BotSettings.WorldTitle} [ON]");
                        CaptureTheFlagBot.SetGodMode(true);
                        CaptureTheFlagBot.Move(new Point(0, 0));
                        CaptureTheFlagBot.SendChatMessage("Connected!");

                        Logger.WriteLog(LogType.EverybodyEditsMessage, "Connected to Everybody Edits succesfully!");
                    }
                    break;
                case EverybodyEditsMessage.PlayerJoinedWorld:
                    {
                        int playerId = m.GetInt(0);
                        string username = m.GetString(1);
                        int smileyId = m.GetInt(3);
                        double xLoc = Math.Round(m.GetDouble(4) / 16.0);
                        double yLoc = Math.Round(m.GetDouble(5) / 16.0);
                        int teamId = m.GetInt(15);

                        JoinedWorld.Players.Add(playerId, new Player(username, smileyId, new Point((int)xLoc, (int)yLoc), TeamHelper.IdToEnum(teamId)));

                        if (PlayersDatabaseTable.Loaded)
                        {
                            if (!JoinedWorld.Players[playerId].IsGuest && !PlayersDatabaseTable.PlayerExists(username))
                            {
                                PlayersDatabaseTable.AddNewPlayer(username);

                                CaptureTheFlagBot.SendPrivateMessage(JoinedWorld.Players[playerId], "Welcome newcomer! Type !help to learn how to play in this world.");
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerLeftWorld:
                    {
                        int playerId = m.GetInt(0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            if (JoinedWorld.Players[playerId].HasEnemyFlag)
                            {
                                CaptureTheFlag.Flags[TeamHelper.GetOppositeTeam(JoinedWorld.Players[playerId].Team)].Return(null, false);
                            }

                            JoinedWorld.Players.Remove(playerId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.SmileyChanged:
                    {
                        int playerId = m.GetInt(0);
                        int smileyId = m.GetInt(1);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            JoinedWorld.Players[playerId].SmileyId = smileyId;

                            if (JoinedWorld.Players[playerId].IsPlayingGame && smileyId == (int)Smiley.Nurse)
                            {
                                CaptureTheFlagBot.SendPrivateMessage(JoinedWorld.Players[playerId], "You are now a healer for your team!");
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.GodModeToggled:
                {
                        int playerId = m.GetInt(0);
                        bool isInGodMode = m.GetBoolean(1);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            JoinedWorld.Players[playerId].IsInGodMode = isInGodMode;

                            AntiCheat.Handle(JoinedWorld.Players[playerId]);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlaceBlock:
                    {
                        int layerId = m.GetInt(0);
                        uint xLoc = m.GetUInt(1);
                        uint yLoc = m.GetUInt(2);
                        int blockId = m.GetInt(3);

                        if (JoinedWorld.Blocks != null)
                        {
                            JoinedWorld.Blocks[layerId, xLoc, yLoc] = new Block(blockId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerMoved:
                    {
                        int playerId = m.GetInt(0);
                        double xLoc = Math.Round(m.GetDouble(1) / 16.0);
                        double yLoc = Math.Round(m.GetDouble(2) / 16.0);
                        int horizontalDir = m.GetInt(7);
                        int verticalDir = m.GetInt(8);
                        bool isPressingSpacebar = m.GetBoolean(9);

                        if (JoinedWorld.Players.ContainsKey(playerId) && JoinedWorld.Players[playerId].IsPlayingGame)
                        {
                            JoinedWorld.Players[playerId].UpdateLocation((int)xLoc, (int)yLoc);
                            JoinedWorld.Players[playerId].HorizontalDirection = (HorizontalDirection)horizontalDir;
                            JoinedWorld.Players[playerId].VerticalDirection = (VerticalDirection)verticalDir;
                            JoinedWorld.Players[playerId].IsPressingSpacebar = isPressingSpacebar;

                            if (!JoinedWorld.Players[playerId].IsRespawning)
                            {
                                CaptureTheFlag.Handle(JoinedWorld.Players[playerId]);
                                RoomEntrances.Handle(JoinedWorld.Players[playerId]);
                                Shop.Handle(JoinedWorld.Players[playerId]);

                                // Handle traps
                                if (JoinedWorld.Players[playerId].CanTriggerTrap)
                                {
                                    foreach (Trap trap in traps)
                                    {
                                        trap.Handle(JoinedWorld.Players[playerId]);
                                    }
                                }

                                // Handle attack/heal system
                                foreach (Player otherPlayer in JoinedWorld.Players.Values)
                                {
                                    if (otherPlayer == JoinedWorld.Players[playerId])
                                    {
                                        continue;
                                    }

                                    AttackSystem.Handle(JoinedWorld.Players[playerId], otherPlayer);
                                    HealingSystem.Handle(JoinedWorld.Players[playerId], otherPlayer);
                                }
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.ChatMessage:
                    {
                        int playerId = m.GetInt(0);
                        string msg = m.GetString(1);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            ChatMessageCommands.Handle(JoinedWorld.Players[playerId], msg);
                        }
                    }
                    break;
                case EverybodyEditsMessage.TeamChanged:
                    {
                        int playerId = m.GetInt(0);
                        int teamId = m.GetInt(1);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            JoinedWorld.Players[playerId].Team = (Team)teamId;

                            AutoBalance.Handle(JoinedWorld.Players[playerId], JoinedWorld.Players);
                        }
                    }
                    break;
                case EverybodyEditsMessage.NonPlayableCharacterBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = m.GetInt(0);
                            int yLoc = m.GetInt(1);
                            int blockId = m.GetInt(2);
                            string name = m.GetString(3);
                            string[] messages = new string[] { m.GetString(4), m.GetString(5), m.GetString(6) };

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new NonPlayableCharacterBlock(blockId, name, messages);
                        }
                    }
                    break;
                case EverybodyEditsMessage.MorphableBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = m.GetInt(0);
                            int yLoc = m.GetInt(1);
                            int blockId = m.GetInt(2);
                            int morphId = m.GetInt(3);
                            int layer = m.GetInt(4);

                            JoinedWorld.Blocks[layer, xLoc, yLoc] = new MorphableBlock(blockId, morphId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.MusicBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = m.GetInt(0);
                            int yLoc = m.GetInt(1);
                            int blockId = m.GetInt(2);
                            int soundId = m.GetInt(3);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new MusicBlock(blockId, soundId);
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerRespawn:
                    {
                        if (m.Count >= 6)
                        {
                            for (uint i = 2; i <= m.Count - 4; i += 4)
                            {
                                int playerId = m.GetInt(i);

                                if (JoinedWorld.Players.ContainsKey(playerId))
                                {
                                    double xLoc = Math.Round(m.GetDouble(i + 1) / 16.0);
                                    double yLoc = Math.Round(m.GetDouble(i + 2) / 16.0);
                                    int deathCount = m.GetInt(i + 3);

                                    JoinedWorld.Players[playerId].UpdateLocation((int)xLoc, (int)yLoc);

                                    if (JoinedWorld.Players[playerId].LastAttacker != null)
                                    {
                                        CaptureTheFlagBot.SendPrivateMessage(JoinedWorld.Players[playerId], $"You were killed by player {JoinedWorld.Players[playerId].LastAttacker.Username}!");
                                        CaptureTheFlagBot.SendPrivateMessage(JoinedWorld.Players[playerId].LastAttacker, $"You killed player {JoinedWorld.Players[playerId].Username}!");

                                        if (PlayersDatabaseTable.Loaded)
                                        {
                                            PlayerDatabaseRow playerData = PlayersDatabaseTable.GetPlayerDatabaseRow(JoinedWorld.Players[playerId].LastAttacker.Username);

                                            if (playerData != null)
                                            {
                                                playerData.TotalKills++;
                                            }
                                        }

                                        JoinedWorld.Players[playerId].LastAttacker = null;

                                        GameFund.Increase(GameFundIncreaseReason.PlayerKilledEnemy);
                                    }

                                    if (deathCount > JoinedWorld.Players[playerId].DeathCount)
                                    {
                                        JoinedWorld.Players[playerId].Respawn();

                                        Team enemyTeam = TeamHelper.GetOppositeTeam(JoinedWorld.Players[playerId].Team);
                                        if (CaptureTheFlag.Flags[enemyTeam].Holder == JoinedWorld.Players[playerId])
                                        {
                                            CaptureTheFlagBot.SendChatMessage($"Player {JoinedWorld.Players[playerId].Username} died while holding {TeamHelper.EnumToString(enemyTeam)} teams flag.");

                                            CaptureTheFlag.Flags[enemyTeam].Return(null, false);
                                        }

                                        CaptureTheFlagBot.RemoveEffects(JoinedWorld.Players[playerId]);
                                    }

                                    JoinedWorld.Players[playerId].DeathCount = deathCount;
                                }
                            }
                        }
                    }
                    break;
                case EverybodyEditsMessage.PlayerTeleported:
                    {
                        int playerId = m.GetInt(0);
                        int xLoc = (int)Math.Round(m.GetInt(1) / 16.0);
                        int yLoc = (int)Math.Round(m.GetInt(2) / 16.0);

                        if (JoinedWorld.Players.ContainsKey(playerId))
                        {
                            JoinedWorld.Players[playerId].UpdateLocation(xLoc, yLoc);
                        }
                    }
                    break;
                case EverybodyEditsMessage.ReloadWorld:
                    {
                        JoinedWorld.Blocks = WorldDeserializer.DeserializeBlocks(m, JoinedWorld.Width, JoinedWorld.Height);
                    }
                    break;
                case EverybodyEditsMessage.SignBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = m.GetInt(0);
                            int yLoc = m.GetInt(1);
                            int blockId = m.GetInt(2);
                            string text = m.GetString(3);
                            int signColour = m.GetInt(4);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new SignBlock(blockId, text, signColour);
                        }
                    }
                    break;
                case EverybodyEditsMessage.WorldPortalBlockPlaced:
                    {
                        if (JoinedWorld.Blocks != null)
                        {
                            int xLoc = m.GetInt(0);
                            int yLoc = m.GetInt(1);
                            int blockId = m.GetInt(2);
                            string targetWorldId = m.GetString(3);
                            int targetSpawnId = m.GetInt(4);

                            JoinedWorld.Blocks[(uint)BlockLayer.Foreground, xLoc, yLoc] = new WorldPortalBlock(blockId, targetWorldId, targetSpawnId);
                        }
                    }
                    break;
            }
        }
    }
}