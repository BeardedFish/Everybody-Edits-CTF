// File Name:     Player.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.Enums;
using Everybody_Edits_CTF.Core.Settings;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.DataStructures
{
    public sealed class Player
    {
        public const int MaxHealth = 100;

        private string _username;

        /// <summary>
        /// States whether the player is playing capture the flag or not.
        /// </summary>
        public bool IsPlayingGame
        {
            get
            {
                return Team != Team.None;
            }
        }

        /// <summary>
        /// States whether this player is a guest or not.
        /// </summary>
        public bool IsGuest
        {
            get => Username.ToLower().StartsWith("guest-");
        }

        /// <summary>
        /// States whether the player is an administrator or not. Administrators are defined in the GameSettings.cs file.
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                for (int i = 0; i < BotSettings.Administrators.Length; i++)
                {
                    if (string.Equals(Username, BotSettings.Administrators[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// States whether the player is on the blue flag or not.
        /// </summary>
        public bool IsOnBlueFlag
        {
            get
            {
                return Location == GameSettings.BlueFlagLocation;
            }
        }

        /// <summary>
        /// States whether the player is on the red flag or not.
        /// </summary>
        public bool IsOnRedFlag
        {
            get
            {
                return Location == GameSettings.RedFlagLocation;
            }
        }

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

        public bool IsRespawning
        {
            get
            {
                return Location.X >= GameSettings.RespawnCooldownZone.X
                    && Location.X <= GameSettings.RespawnCooldownZone.Width
                    && Location.Y >= GameSettings.RespawnCooldownZone.Y
                    && Location.Y <= GameSettings.RespawnCooldownZone.Height;
            }
        }

        /// <summary>
        /// States whether the player is currently in god mode or not.
        /// </summary>
        public bool IsInGodMode
        {
            get;
            set;
        }

        /// <summary>
        /// The smiley id that the player currently has.
        /// </summary>
        public int SmileyId
        {
            get;
            set;
        }

        /// <summary>
        /// The current HP of the player.
        /// </summary>
        public int Health
        {
            get;
            private set;
        } = MaxHealth;

        /// <summary>
        /// The username of the player.
        /// </summary>
        public string Username
        {
            get => _username.ToUpper();
            set => _username = value;
        }

        /// <summary>
        /// States whether the player has the enemy flag or not.
        /// </summary>
        public bool HasEnemyFlag
        {
            get;
            set;
        }

        /// <summary>
        /// The location of the player in the Everybody Edits world.
        /// 
        /// NOTE: This location is not always accurate.
        /// </summary>
        public Point Location
        {
            get;
            set;
        }

        /// <summary>
        /// The team that the player is currently playing for.
        /// </summary>
        public Team Team
        {
            get;
            set;
        }

        /// <summary>
        /// The horizontal direction that the player is currently travelling.
        /// </summary>
        public HorizontalDirection HorizontalDirection
        {
            get;
            set;
        }

        /// <summary>
        /// The vertical direction that the player is currently travelling.
        /// </summary>
        public VerticalDirection VerticalDirection
        {
            get;
            set;
        }

        /// <summary>
        /// The last enemy player that attacked this player.
        /// </summary>
        public Player LastAttacker
        {
            get;
            set;
        } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="smileyId"></param>
        /// <param name="location"></param>
        /// <param name="team"></param>
        public Player(string username, int smileyId, Point location, Team team)
        {
            Username = username;
            SmileyId = smileyId;
            Location = location;
            Team = team;
        }

        /// <summary>
        /// Attacks the player by removing one health point from them.
        /// </summary>
        /// <returns>True if the player died after the attack, if not, false.</returns>
        public void Attack(Player attacker)
        {
            if (Health > 0)
            {
                Health -= 5;

                LastAttacker = attacker;
            }
        }

        public void Die()
        {
            CaptureTheFlagBot.KillPlayer(this);
        }

        /// <summary>
        /// Heals the player by 5 health points.
        /// </summary>
        /// <returns></returns>
        public bool Heal()
        {
            Health += 5;

            return Health >= MaxHealth;
        }

        public void Respawn()
        {
            if (!IsPlayingGame || IsRespawning)
            {
                return;
            }

            Point respawnLocation = Team == Team.Blue ? GameSettings.BlueSpawnLocation : GameSettings.RedSpawnLocation;
            Task.Run(async() =>
            {
                CaptureTheFlagBot.TeleportPlayer(this, GameSettings.DeathSpawnLocation.X, GameSettings.DeathSpawnLocation.Y);

                await Task.Delay(GameSettings.RespawnCooldownMs);

                CaptureTheFlagBot.TeleportPlayer(this, respawnLocation.X, respawnLocation.Y);
            });
        }

        public void RestoreHealth()
        {
            Health = MaxHealth;
        }

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
            if (Team == Team.None || Team == Team.None)
            {
                return false;
            }

            return Team != player.Team;
        }

        public bool IsNearPlayer(Player player)
        {
            const int xOffset = 3;
            const int yOffset = 2;

            int x = Math.Abs(Location.X - player.Location.X);
            int y = Math.Abs(Location.Y - player.Location.Y);

            return x <= xOffset && y <= yOffset;
        }
    }
}