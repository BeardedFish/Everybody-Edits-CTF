using System;
using System.Collections.Generic;
using System.Text;

namespace Everybody_Edits_CTF.Helpers
{
    public static class CommandHelper
    {
        public static bool IsBotCommand(string cmd)
        {
            char[] CMD_PREFIXES = { '.', '>', '!', '#' };

            if (cmd.Length >= 2)
            {
                for (int i = 0; i < CMD_PREFIXES.Length; i++)
                {
                    if (cmd[0] == CMD_PREFIXES[i])
                    {
                        return true;
                    }
                }
            }


            return false;
        }
    }
}