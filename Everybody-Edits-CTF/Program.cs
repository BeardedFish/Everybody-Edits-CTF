// File Name:     Program.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Database;
using Everybody_Edits_CTF.Logging;
using Everybody_Edits_CTF.Security;
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
        /// Main entry point of the program.
        /// </summary>
        static void Main()
        {
            Console.Title = ProgramTitle;
            Console.WriteLine($"{TitleArt}\n");
            Console.WriteLine("Type \"help\" for a list of commands.\n");

            CaptureTheFlagBot ctfBot = new CaptureTheFlagBot();
#nullable enable
            ConnectionInformation? botConnectionInfo = null;
#nullable disable

            bool quitFlag;
            string userInput;

            do
            {
                Console.Write(">> ");

                userInput = Console.ReadLine().Trim();
                quitFlag = string.Equals(userInput, "quit", StringComparison.OrdinalIgnoreCase);

                if (!quitFlag)
                {
                    switch (userInput.ToLower())
                    {
                        case "about":
                            {
                                Console.WriteLine("Capture The Flag Bot - By: Darian Benam");
                                Console.WriteLine("\nOriginally programmed in the year 2015, this application allows the ability for a Capture The Flag game to be played in the flash game Everybody Edits. This application was programmed in C# .NET Framework (v4.7.2) and integrates a MySQL database to save player data.\n");
                                Console.WriteLine("If you are interested in downloading the source code of this application or are interested in the viewing list of features this application offers, visit this link: https://github.com/BeardedFish/Everybody-Edits-CTF/");
                            }
                            break;
                        case "cls":
                        case "clear":
                            {
                                Console.Clear();
                                Console.WriteLine($"{TitleArt}");
                            }
                            break;
                        case "connect":
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
                                        MySqlDatabase.Load();
                                        Console.WriteLine($"[{(MySqlDatabase.Loaded ? "SUCCESS" : $"FAIL")}]");

                                        Console.Write("Connecting to Everybody Edits... ");
                                        Console.WriteLine($"[{(ctfBot.Connect(botConnectionInfo) == null ? "SUCCESS" : $"FAIL")}]");
                                    }
                                    else
                                    {
                                        Console.WriteLine("The bot is already connected!");
                                    }
                                }
                            }
                            break;
                        case "disconnect":
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
                            break;
                        case "help":
                            {
                                Console.WriteLine("about - Displays information about the application.");
                                Console.WriteLine("connect - Connects the bot to Everybody Edits.");
                                Console.WriteLine("disconnect - Disconnects the bot from Everybody Edits.");
                                Console.WriteLine("help - Shows a list of commands on how to use the program.");
                                Console.WriteLine("setup - Sets up the details of the Everybody Edits bot.");
                            }
                            break;
                        case "logs":
                            {
                                Console.WriteLine(Logger.LogText == string.Empty ? "Log text is empty." : Logger.LogText);
                            }
                            break;
                        case "setup":
                            {
                                Console.WriteLine("[!] NOTICE: The information entered here is NOT encrypted or stored securely.\n"); // The values below are stored as plain text in memory

                                Console.Write("Email: ");
                                string botEmail = Console.ReadLine();

                                Console.Write("Password: ");
                                string botPassword = ConsoleSecurity.GetMaskedConsoleInput('*');

                                Console.Write("World Id: ");
                                string worldId = Console.ReadLine();

                                botConnectionInfo = new ConnectionInformation(botEmail, botPassword, worldId);

                                Console.WriteLine("\nSetup process complete! You may now connect the bot to Everybody Edits by typing \"connect\".");
                            }
                            break;
                        default:
                            {
                                Console.WriteLine("Invalid command!");
                            }
                            break;
                    }

                    Console.WriteLine();
                }
            } while (!quitFlag);
        }
    }
}