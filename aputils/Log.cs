using System;
using System.IO;

namespace aputils
{
    public class Log
    {
        public static void DbgLg(string str, string sev = "[INFO]", bool prntLg = false, bool prntVerbose = false, string dir = "C:/Temp/Logs/", string fn = "aputil.log")
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.AppendAllText(dir + fn, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff") + " | " + sev + " |\t " + str + Environment.NewLine);

            if (prntLg && prntVerbose)
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff") + " | " + sev + " | " + str);
            else if (prntLg)
                Console.WriteLine(str);
        }
    }
}
