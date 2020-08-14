// File Name:     AdminCommands.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics;
using Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics.Commands;
using System.Linq;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics.Commands
{
    public sealed class AdminCommands : Command
    {
        public AdminCommands() : base(new string[] { "disconnect", "kick" })
        {

        }

        public override bool Handle(Player player, ParsedCommand parsedCommand)
        {
            if (parsedCommand != null && ValidCommands.Contains(parsedCommand.Command))
            {
                if (player.IsAdmin)
                {
                    switch (parsedCommand.Command)
                    {
                        case "disconnect":
                            {
                                CtfBot.Disconnect();
                            }
                            break;
                        case "kick":
                            {
                                if (player.IsAdmin)
                                {
                                    if (parsedCommand.Parameters.Length >= 1)
                                    {
                                        string playerToKick = parsedCommand.Parameters[0];
                                        string reason = "";

                                        if (parsedCommand.Parameters.Length >= 2)
                                        {
                                            for (int i = 2; i < parsedCommand.Parameters.Length; i++)
                                            {
                                                reason += parsedCommand.Parameters[i] + " ";
                                            }
                                        }

                                        CtfBot.KickPlayer(playerToKick, reason);
                                    }
                                    else
                                    {
                                        CtfBot.SendPrivateMessage(player, $"Insufficient amount of parameters for command.");
                                    }
                                }
                                else
                                {
                                    CtfBot.SendPrivateMessage(player, $"You don't have permission to execute this command.");
                                }
                            }
                            break;
                    }

                    return true;
                }
                else
                {
                    CtfBot.SendPrivateMessage(player, $"You don't have permission to execute this command.");
                }
            }

            return false;
        }
    }
}