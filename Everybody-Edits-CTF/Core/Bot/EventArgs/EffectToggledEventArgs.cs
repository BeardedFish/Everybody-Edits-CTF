// File Name:     EffectToggledEventArgs.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventArgs
{
    public class EffectToggledEventArgs
    {
        public Player Player { get; set; }

        public Effect Effect { get; set; }

        public bool IsEffectEnabled { get; set; }

        public EffectToggledEventArgs(Player player, Effect effect, bool isEnabled)
        {
            Player = player;
            Effect = effect;
            IsEffectEnabled = isEnabled;
        }
    }
}