using System;

namespace Files.Linux
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Files");

            var launcher = new AvaloniaLauncher();
            launcher.Start(Array.Empty<string>());
        }
    }
}