using System;

namespace aputils
{
    public class TextProgressBar
    {
        private static int sAnimationIndex = 0;

        public static void Render(string task, int complete)
        {
            Render(task, complete, 100, 10, '#', '-', @"|/-\", ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.Black, ConsoleColor.Black);
        }

        public static void Render(string task, int complete, int max, int sz, char progTok, char incomTok, string seq, ConsoleColor baseCol, ConsoleColor fillCol, ConsoleColor animCol, ConsoleColor fillBgCol, ConsoleColor baseBgCol)
        {
            int left = Console.CursorLeft;
            decimal perc = complete / (decimal)max;
            int chars = (int)Math.Floor(perc / (1 / (decimal)sz));
            string c1 = string.Empty, c2 = string.Empty;

            for (int i = 0; i < chars; i++) c1 += progTok;
            for (int i = 0; i < sz - chars; i++) c2 += incomTok;

            Console.CursorVisible = false;
            Console.Write(" > " + task + "... ");
            Console.Write("[");
            Console.ForegroundColor = fillCol;
            Console.BackgroundColor = fillBgCol;
            Console.Write(c1);
            Console.ResetColor();
            Console.ForegroundColor = baseCol;
            Console.BackgroundColor = baseBgCol;
            Console.Write(c2);
            Console.ResetColor();
            Console.Write("]");
            Console.Write("  {0}%", (perc * 100).ToString("N0"));
            Console.ForegroundColor = animCol;
            Console.Write(" " + seq[sAnimationIndex++ % seq.Length]);
            Console.ResetColor();
            Console.CursorLeft = left;

            if (complete == max) Console.Write(" > " + task + "... Done." + new string(' ', c1.Length + seq.Length + 1) + '\n');
        }
    }
}
