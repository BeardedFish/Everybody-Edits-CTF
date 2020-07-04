// File Name:     Program.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Sunday, June 28, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.Data;
using Everybody_Edits_CTF.Core.DataStructures;
using PlayerIOClient;
using System;
using System.Data.OleDb;

namespace Everybody_Edits_CTF
{
    class Program
    {
        private const string ProgramTitle = "Everybody Edits - Capture The Flag";
        private const StringComparison CommandCompareCase = StringComparison.OrdinalIgnoreCase;


        /// <summary>
        /// Main entry point of the program.
        /// </summary>
        /// <param name="args">The command line arguments passed to the program.</param>
        static void Main(string[] args)
        {
            Console.Title = ProgramTitle;
            Console.WriteLine($"{ProgramTitle}\n");

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
                        Console.WriteLine($"{ProgramTitle}");
                    }
                    else if (string.Equals(inputCmd, "connect", CommandCompareCase))
                    {
                        Console.Write("Loading players table from MySql database... ");
                        PlayersDatabaseTable.Load();
                        Console.WriteLine($"[{(PlayersDatabaseTable.Loaded ? "SUCCESS" : $"FAIL")}]");
    
                        Console.WriteLine("Connecting to Everybody Edits...");

                        PlayerIOError connectionError;
                        if ((connectionError = CaptureTheFlagBot.Connect()) == null)
                        {
                            Console.WriteLine("Connected!");
                        }
                        else
                        {
                            Console.WriteLine($"Unable to connect to the Everybody Edits world with the ID {GameSettings.WorldId}.");
                            Console.WriteLine($"\nError reason: \"{connectionError.Message}\".");
                        }
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
            } while (!string.Equals(inputCmd, "quit", StringComparison.OrdinalIgnoreCase));
        }
    }
}