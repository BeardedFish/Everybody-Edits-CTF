﻿// File Name:     ShopItem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, July 4, 2020

using System.Drawing;

namespace Everybody_Edits_CTF.Core.Bot.DataContainers
{
    public sealed class ShopItem
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The cost of the item (in coins).
        /// </summary>
        public int Cost { get; private set; }

        /// <summary>
        /// The location in the Everybody Edits world where the player can buy the item.
        /// </summary>
        public Point PurchaseLocation { get; private set; }

        /// <summary>
        /// The location in the Everybody Edits world where the purchasing player will be teleported if a successful purchase is made.
        /// </summary>
        public Point PurchaseTeleportLocation { get; private set; }

        /// <summary>
        /// Constructor for creating a shop item. A shop item can be bought by players. Refer to <see cref="GameMechanics.Shop"/> for more information.
        /// </summary>
        /// <param name="name">Refer to <see cref="Name"/> for description.</param>
        /// <param name="cost">Refer to <see cref="Cost"/> for description.</param>
        /// <param name="purchaseLocation">Refer to <see cref="PurchaseLocation"/> for description.</param>
        /// <param name="purchaseTeleportLocation"><see cref="PurchaseLocation"/></param>
        public ShopItem(string name, int cost, Point purchaseLocation, Point purchaseTeleportLocation)
        {
            Name = name;
            Cost = cost;
            PurchaseLocation = purchaseLocation;
            PurchaseTeleportLocation = purchaseTeleportLocation;
        }
    }
}
