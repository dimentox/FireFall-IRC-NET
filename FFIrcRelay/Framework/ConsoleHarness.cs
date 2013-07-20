using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFIrcRelay
{
    public static class ConsoleHarness
    {
        public static void Run(string[] args, IService service)
        {
            string serviceName = service.GetType().Name;
            bool isRunning = true;
            service.OnStart(args);
            while (isRunning)
            {
                WriteToConsole(ConsoleColor.Yellow, "Enter either [Q]uit, [P]ause, [R]esume : ");
                isRunning = HandleConsoleInput(service, Console.ReadLine());
            }
            service.OnStop();
            service.OnShutdown();
        }
        private static bool HandleConsoleInput(IService service, string line)
        {
            bool canContinue = true;
            if (line != null)
            {
                switch (line.ToUpper())
                {
                    case "Q":
                        canContinue = false;
                        break;

                    case "P":
                        service.OnPause();
                        break;

                    case "R":
                        service.OnContinue();
                        break;

                    default:
                        WriteToConsole(ConsoleColor.Red, "Did not understand that input, try again.");
                        break;
                }
            }
            return canContinue;
        }
        public static void WriteToConsole(ConsoleColor foregroundColor, string format,
              params object[] formatArguments)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(format, formatArguments);
            Console.Out.Flush();

            Console.ForegroundColor = originalColor;
        }
    }
}
