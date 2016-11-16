using System;

namespace aputils
{
    public class AppInfo
    {
        public static void Render()
        {
            Render(System.AppDomain.CurrentDomain.FriendlyName, "", '-', true);
        }

        public static void Render(string app, string dev, char headerChar, bool includeTimeDate)
        {
            int conW = Console.WindowWidth;
            string head = app;

            if (!string.IsNullOrEmpty(dev))
                head = app + " - " + dev;

            string dt = DateTime.Now.ToShortTimeString() + " - " + DateTime.Now.ToShortDateString();

            Console.WriteLine(new string(headerChar, conW));
            Console.WriteLine(head.PadLeft(conW / 2 + head.Length / 2) + "\n");

            if (includeTimeDate)
                Console.WriteLine((dt).PadLeft(conW / 2 + dt.Length / 2) + "\n");

            Console.WriteLine(new string(headerChar, conW));
        }
    }
}
