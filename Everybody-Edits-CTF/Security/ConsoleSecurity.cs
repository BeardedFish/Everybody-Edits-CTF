// File Name:     ConsoleSecurity.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, December 25, 2020

using System;
using System.Text;

namespace Everybody_Edits_CTF.Security
{
    public static class ConsoleSecurity
    {
        /// <summary>
        /// <para>
        /// Masks a user's input when they type in the <see cref="Console"/> and returns the input when the enter key is pressed. When a user types into the console stream,
        /// the character in the first parameter of this static method is written to the console. The actual user input is stored in memory as plain text. This method does
        /// not support arrow key input. This means that you can only append and delete the last character that was entered.
        /// </para>
        /// <para>
        /// Example scenario:
        /// </para>
        /// <para>
        /// If the masking character was '*' and the user typed "Hello, World!" into the console stream then the console output would be "*************" and the value in
        /// memory would be "Hello, World!".
        /// </para>
        /// </summary>
        /// <param name="maskingCharacter">The character that will be printed to the <see cref="Console"/> stream which will mask the user's input.</param>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>A <see cref="string"/> in plain text format which represents the user's input.</returns>
        public static string GetMaskedConsoleInput(char maskingCharacter)
        {
            StringBuilder maskedInputResult = new StringBuilder();
            ConsoleKeyInfo currentPressedKey;
            bool isCurrentKeyEnter;

            do
            {
                currentPressedKey = Console.ReadKey(true);
                isCurrentKeyEnter = currentPressedKey.Key == ConsoleKey.Enter;

                if (!isCurrentKeyEnter)
                {
                    if (currentPressedKey.Key == ConsoleKey.Backspace && maskedInputResult.Length != 0)
                    {
                        // Remove the last asterisk character in the console stream
                        Console.Write("\b \b");

                        // Remove the last character in the the string builder variable
                        maskedInputResult.Remove(maskedInputResult.Length - 1, 1);
                    }
                    else
                    {
                        if (!char.IsControl(currentPressedKey.KeyChar)) // Character is not a control key such as F1, F2, arrow keys, etc.
                        {
                            Console.Write(maskingCharacter);

                            // Add the character to the end of the string builder variable
                            maskedInputResult.Append(currentPressedKey.KeyChar);
                        }
                    }
                }
            } while (!isCurrentKeyEnter);

            // Write a new line to the console stream because the user pressed the enter key
            Console.WriteLine();

            return maskedInputResult.ToString();
        }
    }
}