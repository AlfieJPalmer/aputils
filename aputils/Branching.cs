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

            do
            {
                Console.Write(str + " " + string.Join("/", resp) + " : ");
                input = Console.ReadLine().Trim();
            }
            while (!legalTokens.Any(input.ToUpper().Equals));

            if (input.ToUpper() == "Y" || input.ToUpper() == "YES") return true;
            if (input.ToUpper() == "N" || input.ToUpper() == "NO") return false;

            return false; // theoretically, will never reach here
        }
    }
}
