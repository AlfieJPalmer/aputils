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
            return input.Replace("/", @"\");
        }

    }
}
