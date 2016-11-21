using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using aputils;

namespace TestApp
{
    class Program
    {
        public static void SampleOperation1() { Console.WriteLine("SampleOperation1()"); }
        public static void SampleOperation2() { Console.WriteLine("SampleOperation2()"); }
        public static void SampleOperation3() { Console.WriteLine("SampleOperation3()"); }

        static void Main(string[] args)
        {
            Core.Initialise();
            Core.SetTitle("TestApp");
            Core.SetConsoleBackgroundColour(ConsoleColor.Black);
            Core.SetConsoleFont(5);
            Core.SetConsoleIcon(SystemIcons.Exclamation);

            AppInfo.Render();
            Branching.MultiChoice(new Dictionary<string, Action>()
            {
                { "Operation1", SampleOperation1 },
                { "Operation2", SampleOperation2 },
                { "Operation3", SampleOperation3 },
            });

            if (Branching.Question("Perform Copy?")){
                FileOps.Copy(new FileInfo(@"C:/Temp/file.txt"), new FileInfo(@"C:/Temp/test.txt"), x => TextProgressBar.Render("Copy()", x, 100, 50, '#', '-', @"|/-\", ConsoleColor.DarkGray, ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.Black, ConsoleColor.Black));
            }
            else { Log.Lg("Copy Cancelled"); Core.Exit(); }

            if(Branching.Question("Perform Decompression?")){
                FileOps.CompressDir(@"C:/Temp/folder", @"C:/Temp/dir", x => TextProgressBar.Render("Compress()", x));
            }
            else { Log.Lg("Decompression Cancelled"); }
            Core.Exit();
        }
    }
}
