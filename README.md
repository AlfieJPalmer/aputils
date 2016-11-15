# aputils
C# lib to aid creation of console applications

Example Usage:
```
        static void Main(string[] args)
        {
            aputils.AppInfo.Render();
            for (int i = 0; i < 100; ++i)
            {
                aputils.TextProgressBar.Render("Task1()", i);
                Thread.Sleep(50);
            }
            Console.ReadKey();
        }
```
