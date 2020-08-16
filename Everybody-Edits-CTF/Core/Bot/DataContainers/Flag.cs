// File Name:     Flag.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, July 19, 2020

using Everybody_Edits_CTF.Core.Bot.Deserializer.Blocks;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Helpers;
using System;
using System.Drawing;
using System.Text;

namespace Everybody_Edits_CTF.Core.Bot.DataContainers
{
    public class Flag
    {
        /// <summary>
        /// The team that owns this flag.
        /// </summary>
        public readonly Team OwnerTeam;

        /// <summary>
        /// The home base location for this flag.
        /// </summary>
        public readonly Point HomeLocation;

        /// <summary>
        /// The Everybody Edits morphable block that represents this flag in the game world.
        /// </summary>
        public readonly MorphableBlock Block;

        /// <summary>
        /// The player currently holding this flag.
        /// </summary>
        public Player Holder { get; private set; } = null;

        /// <summary>
        /// The drop location of the flag.
        /// </summary>
        public Point DropLocation { get; private set; } = Point.Empty;

        /// <summary>
        /// States whether the flag has been dropped or not.
        /// </summary>
        public bool IsDropped => DropLocation != Point.Empty;

        /// <summary>
        /// States whether the flag has been taken by an enemy player or not.
        /// </summary>
        public bool IsTaken => Holder != null;

        /// <summary>
        /// States the current location of the flag.
        /// 
        /// If the flag is currently taken, then <see cref="Player.Location"/> is returned. If the flag is dropped, then <see cref="DropLocation"/> is returned. If the flag is not
        /// taken or not dropped, then <see cref="HomeLocation"/> is returned.
        /// </summary>
        public Point CurrentLocation
        {
            get
            {
                if (IsTaken)
                {
                    return Holder.Location;
                }
                else if (IsDropped)
                {
                    return DropLocation;
                }
                else
                {
                    return HomeLocation;
                }
            }
        }

        /// <summary>
        /// The time a player has to wait in order to pickup the flag after it has been dropped, in milliseconds.
        /// </summary>
        private const long PickupCooldownMs = 1500;

        /// <summary>
        /// The tick time the flag was last dropped, in milliseconds.
        /// </summary>
        private long lastDropTick;
        
        /// <summary>
        /// Constructor for building a Flag object.
        /// </summary>
        /// <param name="ownerTeam">The team that this flag belongs to.</param>
        /// <param name="homeLocation">The home base location of the flag.</param>
        /// <param name="block">The block that represents this flag,</param>
        public Flag(Team ownerTeam, Point homeLocation, MorphableBlock block)
        {
            OwnerTeam = ownerTeam;
            HomeLocation = homeLocation;
            Block = block;
        }

        /// <summary>
        /// States whether this flag can be captured by a player or not.
        /// 
        /// A player can capture a flag if:
        ///     - They are currently holding the flag.
        ///     - Their team flag is not dropped.
        ///     - Their team flag is not taken.
        ///     - Their current location is equal to the home location of their base flag.
        /// </summary>
        /// <param name="player">The player to be checked if they can capture the flag or not.</param>
        /// <param name="playerTeamFlag">The flag of the players team.</param>
        /// <returns>True if the player can capture the flag, if not, false.</returns>
        public bool CanBeCapturedBy(Player player, Flag playerTeamFlag)
        {
            return Holder == player && !playerTeamFlag.IsDropped && !playerTeamFlag.IsTaken && player.Location == playerTeamFlag.HomeLocation;
        }

        /// <summary>
        /// States whether a player is able to take a flag or not.
        /// </summary>
        /// <param name="player">The player to check if they can take the flag or not.</param>
        /// <returns>True if the player can take the flag, if not, false.</returns>
        public bool CanBeTakenBy(Player player)
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastDropTick >= PickupCooldownMs && player.Team != OwnerTeam && !IsTaken && player.Location == CurrentLocation;
        }

        /// <summary>
        /// States whether the flag can be returned to base by a player or not.
        /// </summary>
        /// <param name="player">The player to check if they can return the flag or not.</param>
        /// <returns>True if the player can return the flag, if not, false.</returns>
        public bool CanBeReturnedBy(Player player)
        {
            return player.Team == OwnerTeam && IsDropped && player.Location == CurrentLocation;
        }

        /// <summary>
        /// Captures this flag and returns the flag to <see cref="HomeLocation"/>.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        public void Capture(CaptureTheFlagBot ctfBot)
        {
            ctfBot.SayChatMessage($"Player {Holder.Username} has captured the {TeamHelper.EnumToString(OwnerTeam)} team's flag.");

            Return(ctfBot, null, true);
        }

        /// <summary>
        /// Drops the flag in the Everybody Edits worlds on the ground.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        public void Drop(CaptureTheFlagBot ctfBot)
        {
            Point dropLocation = Holder.Location;
            while (dropLocation.Y < ctfBot.JoinedWorld.Blocks.GetLength(2) && ctfBot.JoinedWorld.Blocks[(uint)BlockLayer.Foreground, dropLocation.X, dropLocation.Y + 1].Id == 0)
            {
                dropLocation.Y++;
            }

            ctfBot.PlaceBlock(BlockLayer.Foreground, dropLocation, Block.Id, Block.MorphId);
            ctfBot.SayChatMessage($"Player {Holder.Username} has dropped the {TeamHelper.EnumToString(OwnerTeam)} teams flag.");

            lastDropTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            DropLocation = dropLocation;
            Holder = null;
        }

        /// <summary>
        /// Removes the flag from the <see cref="HomeLocation"/> and sets the <see cref="Holder"/> to the player that is taking the flag.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="taker">The player taking the flag.</param>
        public void Take(CaptureTheFlagBot ctfBot, Player taker)
        {
            ctfBot.PlaceBlock(BlockLayer.Foreground, CurrentLocation, 0);
            ctfBot.SayChatMessage($"Player {taker.Username} has taken the {TeamHelper.EnumToString(OwnerTeam)} teams flag.");

            Holder = taker;
        }

        /// <summary>
        /// Returns the flag to the <see cref="HomeLocation"/>.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="returnee">The player returning the flag.</param>
        /// <param name="flagCaptured">States whether the reason for returning the flag was because the flag was captured or not.</param>
        public void Return(CaptureTheFlagBot ctfBot, Player returnee, bool flagCaptured)
        {
            if (IsDropped)
            {
                ctfBot.PlaceBlock(BlockLayer.Foreground, DropLocation, 0);
            }

            DropLocation = Point.Empty;
            Holder = null;

            ctfBot.PlaceBlock(BlockLayer.Foreground, HomeLocation, Block.Id, Block.MorphId);
            ctfBot.CurrentGameRound.IncreaseGameFund(GameFundIncreaseReason.FlagReturned);

            if (!flagCaptured)
            {
                StringBuilder teamName = new StringBuilder(TeamHelper.EnumToString(OwnerTeam));
                teamName[0] = char.ToUpper(teamName[0]);

                ctfBot.SayChatMessage($"{teamName} teams flag has been returned to base{(returnee == null ? "." : $" by player {returnee.Username}.")}");
            }
        }
    }
}