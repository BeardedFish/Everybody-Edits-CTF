// File Name:     Shop.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Database;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class Shop
    {
        /// <summary>
        /// The Y coordinate where a player can purchase shop items.
        /// </summary>
        private const int PurchaseFloorY = 108;

        /// <summary>
        /// The Y coordinate that a player will be teleported to when they succesfully purchase a shop item.
        /// </summary>
        private const int SuccesfulPurchaseY = 104;

        /// <summary>
        /// Dictionary which defines the string names for effect shop items.
        /// </summary>
        private static readonly Dictionary<Effect, string> effectNameMap = new Dictionary<Effect, string>()
        {
            { Effect.Curse, "curse effect" },
            { Effect.Fly, "fly effect" },
            { Effect.Jump, "higher jump effect" },
            { Effect.Speed, "speed effect" },
            { Effect.Zombie, "zombie effect" },
        };

        /// <summary>
        /// The list of items that the shop offers to players in the Everybody Edits world.
        /// </summary>
        private static readonly List<ShopItem> shopItems = new List<ShopItem>()
        {
            new ShopItem(effectNameMap[Effect.Curse], 30, new Point(232, PurchaseFloorY), new Point(232, SuccesfulPurchaseY)),
            new ShopItem(effectNameMap[Effect.Zombie], 60, new Point(234, PurchaseFloorY), new Point(234, SuccesfulPurchaseY)),
            new ShopItem(effectNameMap[Effect.Jump], 500, new Point(236, PurchaseFloorY), new Point(236, SuccesfulPurchaseY)),
            new ShopItem(effectNameMap[Effect.Speed], 1000, new Point(238, PurchaseFloorY), new Point(238, SuccesfulPurchaseY)),
            new ShopItem("minimap", 3000, new Point(240, PurchaseFloorY), new Point(240, SuccesfulPurchaseY)),
            new ShopItem("mysterious switch", 5000, new Point(242, PurchaseFloorY), new Point(242, SuccesfulPurchaseY)),
            new ShopItem(effectNameMap[Effect.Fly], 10000, new Point(244, PurchaseFloorY), new Point(244, SuccesfulPurchaseY))
        };

        /// <summary>
        /// Handles when a player wants to purchase an item at the shop in the Everybody Edits world. All shop items are defined in the <see cref="shopItems"/> array.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!PlayersDatabaseTable.Loaded || !player.IsPlayingGame || player.IsInGodMode || player.VerticalDirection != VerticalDirection.Down)
            {
                return;
            }

            PlayerDatabaseRow playerData = PlayersDatabaseTable.GetRow(player.Username);
            if (playerData != null)
            {
                foreach (ShopItem shopItem in shopItems)
                {
                    if (player.Location == shopItem.PurchaseLocation) // Player wants to purchase an item
                    {
                        string msgResult = "You don't have enough coins to purchase this item.";

                        if (playerData.Coins >= shopItem.Cost)
                        {
                            playerData.Coins -= shopItem.Cost;
                            msgResult = $"You succesfully bought the {shopItem.Name} for {shopItem.Cost} coin{(playerData.Coins == 1 ? "" : "s")}.";

                            // Set flag variable for the anti-cheat system
                            if (effectNameMap.ContainsValue(shopItem.Name))
                            {
                                player.PurchasedEffectFlag = effectNameMap.FirstOrDefault(x => x.Value == shopItem.Name).Key;
                            }

                            ctfBot.TeleportPlayer(player, shopItem.PurchaseTeleportLocation.X, shopItem.PurchaseTeleportLocation.Y);
                        }

                        ctfBot.SendPrivateMessage(player, msgResult);

                        break; // Player attempted to purchase the item, no need to check the other shop items
                    }
                }
            }
        }
    }
}