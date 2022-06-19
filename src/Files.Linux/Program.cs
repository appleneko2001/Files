using System;
using System.Text;

namespace Files.Linux
{
    // TODO: Complete Linux platform support
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            var result = 0;
            
            Console.WriteLine("Files");
            
            try
            {
                var launcher = new AvaloniaLauncher();
                launcher.Start(args);
            }
            catch (Exception exception)
            {
                var builder = new StringBuilder();
                builder.AppendLine("An critical exception occurred while running application.");
                builder.AppendLine(exception.Message);
                builder.AppendLine();
                builder.AppendLine("Stacktrace:");
                builder.AppendLine(exception.StackTrace);
                builder.AppendLine("Please copy above message and contact developer for report exception.");
                builder.AppendLine("https://github.com/appleneko2001/Files/issues");

                Console.WriteLine(builder.ToString());

                var message = new StringBuilder();
                message.AppendLine("Application has crashed!");
                message.AppendLine("For more information, please look the console output.");

                result = exception.HResult;
            }

            return result;
        }
    }
}