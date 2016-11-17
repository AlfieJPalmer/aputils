// ==============================================================================================================================================================
// Copyright(c) 2016 Alfie J. Palmer

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// ==============================================================================================================================================================

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace aputils
{
    public class Core
    {
        public static bool ConfirmExit { get; set; }

        public static void Initialise()
        {
            SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"SevenZipSharp.dll"));
            if (IntPtr.Size == 8) //x64
                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"7z_x64.dll"));
            else //x86
                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"7z_x86.dll"));

            ConfirmExit = true;
        }

        public static void Exit()
        {
            if (ConfirmExit)
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public static void SetTextForegroundColour(ConsoleColor c)
        {
            Console.ForegroundColor = c;
        }

        public static void SetTextBackgroundColor(ConsoleColor c)
        {
            Console.BackgroundColor = c;
        }

        // Call before writing anything to screen
        public static void SetConsoleBackgroundColour(ConsoleColor c)
        {
            Console.BackgroundColor = c;
            ClearConsole();
        }

        public static void ClearConsole()
        {
            Console.Clear();
        }

        public static void SetTitle(string str)
        {
            Console.Title = str;
        }

        // Stuff from kernel32 dll
        [DllImport("kernel32")]
        public static extern bool SetConsoleIcon(IntPtr hIcon);

        public static bool SetConsoleIcon(Icon icon)
        {
            return SetConsoleIcon(icon.Handle);
        }

        // Default console is not a rich text interface,
        // Read possible fonts as a ConsoleFontIndex from 
        // the ConsoleFontTable, SIZE: { X, Y }
        // 0: X=100, Y=50
        // 1: X=66, Y=37
        // 2: X=50, Y=37
        // 3: X=25, Y=37
        // 4: X=80, Y=25
        // 5: X=57, Y=25
        // 6: X=50, Y=25
        // 7: X=25, Y=25
        // 8: X=33, Y=18
        // 9: X=40, Y=16
        [DllImport("kernel32")]
        private extern static bool SetConsoleFont(IntPtr hOutput, uint index);

        private enum StdHandle
        {
            OutputHandle = -11
        }

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        public static bool SetConsoleFont(uint index)
        {
            return SetConsoleFont(GetStdHandle(StdHandle.OutputHandle), index);
        }
    }
}
