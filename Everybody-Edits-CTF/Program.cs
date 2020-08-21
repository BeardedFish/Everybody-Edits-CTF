// File Name:     Program.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Database;
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
        private const string TitleArt = "=+=...............................................................................................=+=\n" +
                                        "...........~#___________..___________..___________......~#___________..___________..___________......\n" +
                                        "..........~#/ _________/./____  ____/./ _________/.....~#/ _____    /./ _______  /./____  ____/......\n" +
                                        ".........~#/ /.............../ /...../ /________......~#/ /____/ __/./ /      / /....../ /...........\n" +
                                        "........~#/ /.............../ /...../ _________/.....~#/ ______ /_../ /      / /....../ /............\n" +
                                        ".......~#/ /_________....../ /...../ /..............~#/ /_____/  /./ /______/ /....../ /.............\n" +
                                        "......~#/___________/...../_/...../_/..............~#/__________/./__________/....../_/..............\n" +
                                        "=+=...............................................................................................=+=";

        /// <summary>
        /// The case which commands will be compared to.
        /// </summary>
        private const StringComparison CommandCompareCase = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Main entry point of the program.
        /// </summary>
        static void Main()
        {
            CaptureTheFlagBot ctfBot = new CaptureTheFlagBot();
            ConnectionInformation botConnectionInfo = null;

            Console.Title = ProgramTitle;
            Console.WriteLine($"{TitleArt}\n");

            Console.WriteLine("Type \"help\" for a list of commands.\n");

            string userInput;
            do
            {
                Console.Write(">> ");
                userInput = Console.ReadLine().Trim();

                if (!string.Equals(userInput, "quit", CommandCompareCase))
                {
                    if (string.Equals(userInput, "cls", CommandCompareCase) || string.Equals(userInput, "clear", CommandCompareCase))
                    {
                        Console.Clear();
                        Console.WriteLine($"{TitleArt}");
                    }
                    else if (string.Equals(userInput, "connect", CommandCompareCase))
                    {
                        if (botConnectionInfo == null)
                        {
                            Console.WriteLine("Can't connect because the bot connection information has not been setup! Type \"setup\" to setup the bot connection information.");
                        }
                        else
                        {
                            if (!ctfBot.Connected)
                            {
                                Console.Write("Loading players table from MySql database... ");
                                PlayersTable.Load();
                                Console.WriteLine($"[{(PlayersTable.Loaded ? "SUCCESS" : $"FAIL")}]");

                                Console.Write("Connecting to Everybody Edits... ");
                                Console.WriteLine($"[{(ctfBot.Connect(botConnectionInfo) == null ? "SUCCESS" : $"FAIL")}]");
                            }
                            else
                            {
                                Console.WriteLine("The bot is already connected!");
                            }
                        }
                    }
                    else if (string.Equals(userInput, "disconnect", CommandCompareCase))
                    {
                        if (ctfBot.Connected)
                        {
                            Console.WriteLine("Disconnecting...");

                            ctfBot.Disconnect();
                        }
                        else
                        {
                            Console.WriteLine("The bot is not connected!");
                        }
                    }
                    else if (string.Equals(userInput, "help", CommandCompareCase))
                    {
                        Console.WriteLine("about - Shows information about the program.");
                        Console.WriteLine("connect - Connects the bot to Everybody Edits.");
                        Console.WriteLine("disconnect - Disconnects the bot from Everybody Edits.");
                        Console.WriteLine("help - Shows a list of commands on how to use the program.");
                    }
                    else if (string.Equals(userInput, "logs", CommandCompareCase))
                    {
                        Console.WriteLine(Logger.LogText == string.Empty ? "Log text is empty." : Logger.LogText);
                    }
                    else if (string.Equals(userInput, "setup", CommandCompareCase))
                    {
                        Console.WriteLine("[!] NOTE: The information entered here is NOT encrypted or stored securely.\n");

                        Console.Write("Email: ");
                        string botEmail = Console.ReadLine();

                        Console.Write("Password: ");
                        string botPassword = Console.ReadLine();

                        Console.Write("World Id: ");
                        string worldId = Console.ReadLine();

                        botConnectionInfo = new ConnectionInformation(botEmail, botPassword, worldId);

                        Console.WriteLine("\nSetup process complete! You may now connect the bot to Everybody Edits by typing \"connect\".");
                    }
                    else
                    {
                        Console.WriteLine("Invalid command!");
                    }

                    Console.WriteLine();
                }
            } while (!string.Equals(userInput, "quit", CommandCompareCase));
        }
    }
}