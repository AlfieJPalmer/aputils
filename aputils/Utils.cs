using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aputils
{
    class Utils
    {

        public static string ConvertPath(string input)
        {
            if (input.Contains("/"))
                input = input.Replace("/", @"\");
            else if (input.Contains(@"\"))
                input = input.Replace(@"\", "/");

            return input;
        }

        public static void ClearInvalidSelection(int cursorX, int cursorY)
        {
            // Move cursor to beginning of input
            Console.CursorLeft = cursorX;
            Console.CursorTop = cursorY;

            // Clear Line
            Console.Write(new string(' ', Console.WindowWidth));

            // Reset Cursor
            Console.CursorLeft = cursorX;
            Console.CursorTop = cursorY;
        }

    }
}
