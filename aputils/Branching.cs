using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aputils
{
    class Branching
    {
        public static bool Question(string str)
        {
            string[] resp = { "Y", "N" };
            string[] additional = { "YES", "NO" };

            string[] legalTokens = resp.Concat(additional).ToArray();
            string input = string.Empty;

            Console.Write(str + " " + string.Join("/", resp) + " : ");

            int curLeft = Console.CursorLeft;
            int curTop = Console.CursorTop;

            bool invInputFlag = false;
            do
            {
                if (invInputFlag)
                    Utils.ClearInvalidSelection(curLeft, curTop);
                
                input = Console.ReadLine().Trim();
                invInputFlag = true;
            }
            while (!legalTokens.Any(input.ToUpper().Equals));

            if (input.ToUpper() == "Y" || input.ToUpper() == "YES") return true;
            if (input.ToUpper() == "N" || input.ToUpper() == "NO") return false;

            return false; // theoretically, will never reach here
        }

        public static void MultiChoice(Dictionary<string, Action> funcs) // todo handle exceptions here
        {
            int count = 0;
            int selection = -1;

            Console.WriteLine("Make a selection:\n");

            count = 0;
            foreach (KeyValuePair<string, Action> s in funcs)
            {
                ++count;
                Console.WriteLine(" " + count + ". " + s.Key);
            }

            // Prompt User Input
            Console.Write("\n> ");

            int curLeft = Console.CursorLeft;
            int curTop = Console.CursorTop;

            bool invInputFlag = false;
            do
            {
                if (invInputFlag)
                    Utils.ClearInvalidSelection(curLeft, curTop);
                
                string str = Console.ReadLine();
                Int32.TryParse(str, out selection);
                invInputFlag = true;
            } while (selection == 0 || selection > funcs.Count);

            count = 0;
            foreach (KeyValuePair<string, Action> s in funcs)
            {
                ++count;
                if (selection == count)
                    s.Value();
            }
        }
    }
}
