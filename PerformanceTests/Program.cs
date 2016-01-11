using System;
using ECS.PerformanceTests.Measurements;

namespace ECS.PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Setting up measurements . . .");
            Measurement[] measurements =
            {
                new ProcessSystems()
            };
            Console.WriteLine("Setup done");
            Console.WriteLine();

            ConsoleColor defaultForegroundColor = Console.ForegroundColor;
            ConsoleColor nameForegroundColor = ConsoleColor.Green;
            foreach (var measurement in measurements)
            {
                Console.Write("Measure ");
                Console.ForegroundColor = nameForegroundColor;
                Console.WriteLine(measurement.Name);
                Console.ForegroundColor = defaultForegroundColor;
                Console.WriteLine(measurement.Go());
                Console.WriteLine();
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey();
            }
        }
    }
}
