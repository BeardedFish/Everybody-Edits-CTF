// File Name:     Player.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Bot.GameMechanics;
using Everybody_Edits_CTF.Core.Settings;
using Everybody_Edits_CTF.Helpers;
using PlayerIOClient;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.DataStructures
{
    public sealed class Player
    {
        /// <summary>
        /// States whether the player is playing capture the flag or not.
        /// </summary>
        public bool IsPlayingGame => Team != Team.None;

        /// <summary>
        /// States whether this player is a guest or not.
        /// </summary>
        public bool IsGuest => Username.ToLower().StartsWith("guest-");

        /// <summary>
        /// States whether the player is an administrator or not. Administrators are defined in the GameSettings.cs file.
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                foreach (string admin in BotSettings.Administrators)
                {
                    if (string.Equals(Username, admin, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// States whether this player can trigger a trap or not.
        /// 
        /// A player can trigger a trap if:
        ///     - They are not in God mode.
        ///     - They are playing the Capture The Flag game (either or team blue or team red).
        ///     - Their vertical direction is equal to down.
        /// </summary>
        public bool CanTriggerTrap => !IsInGodMode && IsPlayingGame && VerticalDirection == VerticalDirection.Down;

        /// <summary>
        /// The number of times the player died in the Everybody Edits world.
        /// </summary>
        public int DeathCount { get; set; }

        /// <summary>
        /// States whether the player is currently holding the enemy flag or not.
        /// </summary>
        public bool HasEnemyFlag => IsPlayingGame ? CtfGameRound.FlagSystem.Flags[TeamHelper.GetOppositeTeam(Team)].Holder == this : false;

        /// <summary>
        /// States whether the player is in the blue teams base or not.
        /// </summary>
        public bool IsInBlueBase
        {
            get
            {
                const int leftX = 19;
                const int rightX = 45;

                return Location.X >= leftX && Location.X <= rightX;
            }
        }

        /// <summary>
        /// States whether the player is in the red teams base or not.
        /// </summary>
        public bool IsInRedBase
        {
            get
            {
                const int leftX = 354;
                const int rightX = 380;

                return Location.X >= leftX && Location.X <= rightX;
            }
        }

        /// <summary>
        /// States whether the player is currently pressing the spacebar or not.
        /// </summary>
        public bool IsPressingSpacebar { get; set; }

        /// <summary>
        /// States whether the play is respawning in a respawn cooldown zone or not.
        /// </summary>
        public bool IsRespawning => Location == GameSettings.BlueRespawnCooldownLocation || Location == GameSettings.RedRespawnCooldownLocation;

        /// <summary>
        /// States whether the player is currently in God mode or not.
        /// </summary>
        public bool IsInGodMode { get; set; }

        /// <summary>
        /// States whether the player can toggle God mode or not.
        /// </summary>
        public bool CanToggleGodMode { get; set; }

        /// <summary>
        /// The smiley id that the player currently has.
        /// </summary>
        public int SmileyId { get; set; }

        /// <summary>
        /// The current HP of the player.
        /// </summary>
        public int Health { get; private set; } = MaxHealth;

        /// <summary>
        /// The username of the player. The string returned is CAPITALIZED.
        /// </summary>
        public string Username
        {
            get => _username.ToUpper();
            set => _username = value;
        }

        private string _username;

        /// <summary>
        /// The location of the player in the Everybody Edits world. This location is not always accurate because Everybody Edits only reports the location of a player
        /// when they initially press a movement key or when they release a movement key.
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// The team that the player is currently playing for.
        /// </summary>
        public Team Team { get; set; }

        /// <summary>
        /// The horizontal direction that the player is currently travelling.
        /// </summary>
        public HorizontalDirection HorizontalDirection { get; set; }

        /// <summary>
        /// The vertical direction that the player is currently travelling.
        /// </summary>
        public VerticalDirection VerticalDirection { get; set; }

        /// <summary>
        /// The last enemy player that attacked this player.
        /// </summary>
        public Player LastAttacker { get; set; } = null;

        /// <summary>
        /// The maximum health points a player can have.
        /// </summary>
        private const int MaxHealth = 100;

        /// <summary>
        /// The amount of health points a player can gain/lose when touched by either a enemy or a team mate.
        /// </summary>
        private const int AttackHealHealthAmount = 5;

        /// <summary>
        /// Constructor for creating a <see cref="Player"/> object which contains data about an Everybody Edits player.
        /// </summary>
        /// <param name="username">Refer to <see cref="Username"/> for description.</param>
        /// <param name="smileyId">Refer to <see cref="SmileyId"/> for description.</param>
        /// <param name="location">Refer to <see cref="Location"/> for description.</param>
        /// <param name="isInGodMode">Refer to <see cref="IsInGodMode"/> for description.</param>
        /// <param name="team">Refer to <see cref="Team"/> for description.</param>
        /// <param name="canToggleGodMode">Refer to <see cref="CanToggleGodMode"/> for description.</param>
        public Player(string username, int smileyId, Point location, bool isInGodMode, Team team, bool canToggleGodMode)
        {
            Username = username;
            SmileyId = smileyId;
            Location = location;
            IsInGodMode = isInGodMode;
            Team = team;
            CanToggleGodMode = canToggleGodMode;
        }

        /// <summary>
        /// Attacks the player by removing one health point from them.
        /// </summary>
        /// <returns>True if the player died after the attack, if not, false.</returns>
        public void Attack(Player attacker)
        {
            if (Health > 0)
            {
                Health -= AttackHealHealthAmount;

                LastAttacker = attacker;
            }
        }

        /// <summary>
        /// Tells the bot to kill this player.
        /// </summary>
        public void Die()
        {
            CtfBot.KillPlayer(this);
        }

        /// <summary>
        /// Heals the player by 5 health points.
        /// </summary>
        /// <returns></returns>
        public bool Heal()
        {
            Health += AttackHealHealthAmount;

            return Health >= MaxHealth;
        }

        /// <summary>
        /// Respawns the player to their teams respawn location. When respawning, the player has to wait <see cref="GameSettings.RespawnCooldownMs"/>. If the players team
        /// is <see cref="Team.None"/> then this method does nothing. 
        /// </summary>
        public void Respawn()
        {
            if (!IsPlayingGame || IsRespawning)
            {
                return;
            }

            RestoreHealth();

            Task.Run(async() =>
            {
                Point respawnCooldownLocation = Team == Team.Blue ? GameSettings.BlueRespawnCooldownLocation : GameSettings.RedRespawnCooldownLocation;
                CtfBot.TeleportPlayer(this, respawnCooldownLocation.X, respawnCooldownLocation.Y);

                await Task.Delay(GameSettings.RespawnCooldownMs);

                Point respawnLocation = Team == Team.Blue ? GameSettings.BlueCheckpointLocation : GameSettings.RedCheckpointLocation;
                CtfBot.TeleportPlayer(this, respawnLocation.X, respawnLocation.Y);
            });
        }

        /// <summary>
        /// Sets the players health to <see cref="MaxHealth"/>.
        /// </summary>
        public void RestoreHealth()
        {
            Health = MaxHealth;
        }

        /// <summary>
        /// Teleports the player to the lobby.
        /// </summary>
        public void GoToLobby()
        {
            CtfBot.ResetPlayer(this);
        }

        public void UpdateMovementInformation(Message message)
        {
            if (message.Type == EverybodyEditsMessage.PlayerMoved || message.Type == EverybodyEditsMessage.PlayerTeleported)
            {
                Location = new Point((int)Math.Round(message.GetDouble(1) / 16.0), (int)Math.Round(message.GetDouble(2) / 16.0));

                if (message.Type == EverybodyEditsMessage.PlayerMoved)
                {
                    HorizontalDirection = (HorizontalDirection)message.GetInt(7);
                    VerticalDirection = (VerticalDirection)message.GetInt(8);
                    IsPressingSpacebar = message.GetBoolean(9);
                }
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Updates the players <see cref="Location"/> to a specified x and y coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the player.</param>
        /// <param name="y">The y coordinate of the player.</param>
        public void UpdateLocation(int x, int y)
        {
            Location = new Point(x, y);
        }

        /// <summary>
        /// States whether this player is enemies with another player based on their teams. Players with the same teams are allies, while players with different teams are considered
        /// enemies. Players not on a team are not considered enemies with other teams.
        /// </summary>
        /// <param name="player">The player to compare to this player object.</param>
        /// <returns>True if this player is enemies with the player in the parameters, if not, false.</returns>
        public bool IsEnemiesWith(Player player)
        {
            if (Team == Team.None || player.Team == Team.None)
            {
                return false;
            }

            return Team != player.Team;
        }

        /// <summary>
        /// States whether this player is near another player or not. Refer to the method body code for the horizontal and vertical offset values.
        /// </summary>
        /// <param name="player">The player to be compared to this player in terms of distance.</param>
        /// <returns>True if the player is near this</returns>
        public bool IsNearPlayer(Player player)
        {
            const int xOffset = 3; // 2 blocks horizontally
            const int yOffset = 2; // 1 block vertically

            return Math.Abs(Location.X - player.Location.X) <= xOffset && Math.Abs(Location.Y - player.Location.Y) <= yOffset;
        }
    }
}