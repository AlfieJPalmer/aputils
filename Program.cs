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

            FileOps.ReadFirstBytes(@"C:\Temp\equity_euro.px", 4096);
            Console.WriteLine("\n\n---------------------------------------\n\n");
            FileOps.ReadLastBytes(@"C:\Temp\equity_euro.px", 4096);
            Console.ReadKey();
            
            
            //FileOps.CompressDir(@"C:/Temp/Infragistics2013.2", @"C:/Temp/Test3", x => TextProgressBar.Render("Compress()", x));
            //FileOps.CompressFile("C:/Temp/equity_euro.px", "C:/Temp/Test2", x => TextProgressBar.Render("Cmp()", x), SevenZip.OutArchiveFormat.SevenZip);
            //FileOps.CompressFiles(new string[] { @"C:/Temp/equity_euro.px", @"C:/Temp/equity_euro_reinterpret.px" }, @"C:/Temp/Test", x => TextProgressBar.Render("CmpFls()", x), SevenZip.OutArchiveFormat.SevenZip);

            //FileOps.Decompress(new string[] { @"C:/Temp/equity_euro.7z" }, @"C:/Temp/Test4", x => TextProgressBar.Render("Decompress()", x));
            //FileOps.Decompress(@"C:/Temp/test2.zip", x => TextProgressBar.Render("Dmprs()", x));
            //FileOps.Decompress(new string[] { @"C:/Temp/test.px.gz", @"C:/Temp/test2.px.gz"}, x => TextProgressBar.Render("Dmprs()", x));

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
                FileOps.Copy(new FileInfo(@"C:/Temp/equity_euro.px"), new FileInfo(@"C:/Temp/test.px"), x => TextProgressBar.Render("Copy()", x, 100, 50, '#', '-', @"|/-\", ConsoleColor.DarkGray, ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.Black, ConsoleColor.Black));
            }
            else { Log.Lg("Copy Cancelled"); Core.Exit(); }

            if(Branching.Question("Perform Decompression?")){
                FileOps.CompressDir(@"C:/Temp/Infragistics2013.2", @"C:/Temp/test", x => TextProgressBar.Render("Compress()", x)); // fix ext, cmprs dir
            }
            else { Log.Lg("Decompression Cancelled"); }

            Core.Exit();
        }
    }
}
