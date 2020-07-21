// File Name:     Program.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Logging;
using System;

namespace Everybody_Edits_CTF
{
    class Program
    {
        /// <summary>
        /// The title of the console window.
        /// </summary>
        private const string ProgramTitle = "Everybody Edits - Capture The Flag";
        
        /// <summary>
        /// The art text that will appear at very beginning of the console input/output area.
        /// </summary>
        private const string TitleArt = "=+=..............................................................................................=+=\n" +
                                        "..........~#___________..___________..___________......~#___________..___________..___________.....\n" +
                                        ".........~#/ _________/./____  ____/./ _________/.....~#/ _____    /./ _______  /./____  ____/.....\n" +
                                        "........~#/ /.............../ /...../ /________......~#/ /____/ __/./ /      / /....../ /..........\n" +
                                        ".......~#/ /.............../ /...../ _________/.....~#/ ______ /_../ /      / /....../ /...........\n" +
                                        "......~#/ /_________....../ /...../ /..............~#/ /_____/  /./ /______/ /....../ /............\n" +
                                        ".....~#/___________/...../_/...../_/..............~#/__________/./__________/....../_/.............\n" +
                                        "=+=..............................................................................................=+=";

        /// <summary>
        /// The case which commands will be compared to.
        /// </summary>
        private const StringComparison CommandCompareCase = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Main entry point of the program.
        /// </summary>
        /// <param name="args">The command line arguments passed to the program.</param>
        static void Main(string[] args)
        {
            Console.Title = ProgramTitle;
            Console.WriteLine($"{TitleArt}\n");

            Console.WriteLine("Type \"help\" for a list of commands.\n");

            string inputCmd;
            do
            {
                Console.Write("Enter a command: ");
                inputCmd = Console.ReadLine().Trim();

                if (!string.Equals(inputCmd, "quit", CommandCompareCase))
                {
                    if (string.Equals(inputCmd, "about", CommandCompareCase))
                    {
                        Console.WriteLine("Version 1.0");
                        Console.WriteLine("By: Darian Benam");
                    }
                    else if (string.Equals(inputCmd, "cls", CommandCompareCase) || string.Equals(inputCmd, "clear", CommandCompareCase))
                    {
                        Console.Clear();
                        Console.WriteLine($"{TitleArt}");
                    }
                    else if (string.Equals(inputCmd, "connect", CommandCompareCase))
                    {
                        Console.Write("Loading players table from MySql database... ");
                        PlayersDatabaseTable.Load();
                        Console.WriteLine($"[{(PlayersDatabaseTable.Loaded ? "SUCCESS" : $"FAIL")}]");
    
                        Console.Write("Connecting to Everybody Edits... ");
                        Console.WriteLine($"[{(CaptureTheFlagBot.Connect() == null ? "SUCCESS" : $"FAIL")}]");
                    }
                    else if (string.Equals(inputCmd, "disconnect", CommandCompareCase))
                    {
                        if (CaptureTheFlagBot.Connected)
                        {
                            Console.WriteLine("Disconnecting...");

                            CaptureTheFlagBot.Disconnect();
                        }
                        else
                        {
                            Console.WriteLine("The bot is not connected!");
                        }
                    }
                    else if (string.Equals(inputCmd, "help", CommandCompareCase))
                    {
                        Console.WriteLine("about - Shows information about the program.");
                        Console.WriteLine("connect - Connects the bot to Everybody Edits.");
                        Console.WriteLine("disconnect - Disconnects the bot from Everybody Edits.");
                        Console.WriteLine("help - Shows a list of commands on how to use the program.");
                    }
                    else if (string.Equals(inputCmd, "logs", CommandCompareCase))
                    {
                        string outputLogTxt = Logger.LogText == string.Empty ? "Log text is empty." : Logger.LogText;

                        Console.WriteLine("=== LOGS ==================");
                        Console.WriteLine(outputLogTxt);
                        Console.WriteLine("===========================");
                    }
                    else if (string.Equals(inputCmd, "users", CommandCompareCase))
                    {
                        if (CaptureTheFlagBot.Connected)
                        {
                            foreach (Player player in CaptureTheFlagBot.PlayersInWorld.Values)
                            {
                                Console.WriteLine($"{player.Username}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("The bot is not connected!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid command!");
                    }

                    Console.WriteLine();
                }
            } while (!string.Equals(inputCmd, "quit", CommandCompareCase));
        }
    }
}