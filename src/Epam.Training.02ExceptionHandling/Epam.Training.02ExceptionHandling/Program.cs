using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Epam.Training._02ExceptionHandling
{
    class Program
    {
        static void Main()
        {
            SetCulturePattern();
            Settings();
            WriteGreetings();
            Console.WriteLine("Type the text, to finish press ctrl+Enter or shift+Enter:");
            while (true)
            {
                DisplayFirstLetter(ReadText());
                Console.WriteLine();
                Console.WriteLine();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static string ReadText()
        {
            var builder = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey();
                CheckExit(key);
                if (CheckFinishType(key))
                    break;
                if (CheckNewLine(key))
                {
                    builder.Append(Environment.NewLine);
                    continue;
                }
                if (CheckRemoveSymbol(key))
                {
                    builder.Remove(builder.Length - 1, 1);
                    continue;
                }
                if (CheckLetterOrDigit(key))
                    builder.Append(key.KeyChar);
            }

            return builder.ToString();
        }

        private static void DisplayFirstLetter(string text)
        {
            Console.WriteLine("First letters of each line:");
            foreach (var line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                try
                {
                    Console.WriteLine(line[0]);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Blank line");
                }
            }
        }

        private static void SetCulturePattern()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        }

        private static void Settings()
        {
            Console.Title = "Generate reports tool";

            //Console.SetWindowPosition(0, 0);   // sets window position to upper left
            //Console.SetBufferSize(120, 100);   // make sure buffer is bigger than window
            //Console.SetWindowSize(119, 54);   //set window size to almost full screen 
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
        }

        private static void WriteGreetings()
        {
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║                                              ║");
            Console.WriteLine("║                Epam Systems                  ║");
            Console.WriteLine("║         TASK #2 EXCEPTION HANDLING           ║");
            Console.WriteLine("║                                              ║");
            Console.WriteLine("║                  written by                  ║");
            Console.WriteLine("║         <ihar_salzhanitsyn@epam.com>         ║");
            Console.WriteLine("║                                              ║");
            Console.WriteLine("║                      2017                    ║");
            Console.WriteLine("║                                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝ ");
            Console.Write("\r\n\r\n\r\n");
            Console.WriteLine("Please press ESC to exit the program.");
            Console.Write("\r\n\r\n");
        }

        private static void CheckExit(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Push any key to exit...");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private static bool CheckFinishType(ConsoleKeyInfo key)
        {
            return key.Key == ConsoleKey.Enter &&
                   (key.Modifiers == ConsoleModifiers.Shift ||
                    key.Modifiers == ConsoleModifiers.Control);
        }

        private static bool CheckNewLine(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter)
            {
                Console.CursorTop = Console.CursorTop + 1;
                return true;
            }
            return false;
        }

        private static bool CheckRemoveSymbol(ConsoleKeyInfo key)
        {
            return key.Key == ConsoleKey.Backspace;
        }

        private static bool CheckLetterOrDigit(ConsoleKeyInfo key)
        {
            if (Char.IsLetterOrDigit(key.KeyChar))
                return true;

            Console.CursorLeft = Console.CursorLeft - 1;
            return false;
        }

    }
}
