// File Name:     EffectToggledEventArgs.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventArgs
{
    public class EffectToggledEventArgs
    {
        /// <summary>
        /// The player who received/lost the effect.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// The effect the player received/lost.
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// States whether the effect was enabled for the player or not.
        /// </summary>
        public bool IsEffectEnabled { get; set; }

        /// <summary>
        /// Constructor for a <see cref="EffectToggledEventArgs"/> object which stores data about when a player receives/loses an effect in Everybody Edits.
        /// </summary>
        /// <param name="player">Refer to <see cref="Player"/> for description.</param>
        /// <param name="effect">Refer to <see cref="Effect"/> for description.</param>
        /// <param name="isEffectEnabled">Refer to <see cref="IsEffectEnabled"/> for description.</param>
        public EffectToggledEventArgs(Player player, Effect effect, bool isEffectEnabled)
        {
            Player = player;
            Effect = effect;
            IsEffectEnabled = isEffectEnabled;
        }
    }
}