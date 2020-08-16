// File Name:     PlayerResetEventArgs.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using System.Collections.Generic;

namespace Everybody_Edits_CTF.Core.Bot.EventArgs
{
    public class PlayerResetEventArgs
    {
        public bool PropertiesReset { get; set; }

        public List<Player> PlayersReset { get; set; }

        public PlayerResetEventArgs(bool propertiesReset, List<Player> playersReset)
        {
            PropertiesReset = propertiesReset;
            PlayersReset = playersReset;
        }
    }
}