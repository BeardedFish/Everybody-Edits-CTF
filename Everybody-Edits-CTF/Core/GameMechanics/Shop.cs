// File Name:     Shop.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, July 4, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Data;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Enums;
using System.Collections.Generic;
using System.Drawing;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class Shop
    {
        private const int PurchaseFloorY = 108;
        private const int SuccesfulPurchaseY = 104;

        /// <summary>
        /// The list of items that the shop offers to players in the Everybody Edits world.
        /// </summary>
        private static readonly List<ShopItem> ShopItems = new List<ShopItem>()
        {
            new ShopItem("curse effect", 30, new Point(232, PurchaseFloorY), new Point(232, SuccesfulPurchaseY)),
            new ShopItem("zombie effect", 60, new Point(234, PurchaseFloorY), new Point(234, SuccesfulPurchaseY)),
            new ShopItem("higher jump effect", 500, new Point(236, PurchaseFloorY), new Point(236, SuccesfulPurchaseY)),
            new ShopItem("speed effect", 750, new Point(238, PurchaseFloorY), new Point(238, SuccesfulPurchaseY)),
            new ShopItem("fly effect", 3500, new Point(240, PurchaseFloorY), new Point(240, SuccesfulPurchaseY))
        };

        /// <summary>
        /// Handles a player if there located in the shop.
        /// </summary>
        /// <param name="player">The player to handle in the shop.</param>
        public static void Handle(Player player)
        {
            if (!PlayersDatabaseTable.Loaded || player.VerticalDirection != VerticalDirection.Down)
            {
                return;
            }

            PlayerDatabaseRow playerData = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);
            if (playerData != null)
            {
                foreach (ShopItem shopItem in ShopItems)
                {
                    if (player.Location == shopItem.PurchaseLocation) // Player wants to purchase an item
                    {
                        string msgResult = "You don't have enough coins to purchase this item.";

                        if (playerData.Coins >= shopItem.Cost)
                        {
                            playerData.Coins -= shopItem.Cost;
                            msgResult = $"You succesfully bought the {shopItem.Name} for {shopItem.Cost} coin{(playerData.Coins == 1 ? "" : "s")}";
                            
                            CaptureTheFlagBot.TeleportPlayer(player, shopItem.PurchaseTeleportLocation.X, shopItem.PurchaseTeleportLocation.Y);
                        }

                        CaptureTheFlagBot.SendPrivateMessage(player, msgResult);

                        break;
                    }
                }
            }
        }
    }
}