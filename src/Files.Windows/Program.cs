using System;
using System.Text;
using System.Threading;
using Files.Windows.Services.Native;
using Files.Windows.Services.Native.Enums;

namespace Files.Windows
{
    internal static class Program
    {
        private const string AppName = "Files";
        
        [STAThread]
        private static int Main(string[] args)
        {
            var result = 0;
            var ctx = new CancellationTokenSource();
            AvaloniaLauncher? launcher = null;

            Console.WriteLine(AppName);

            try
            {
                launcher = new AvaloniaLauncher();
                launcher.Start(args, ctx.Token);
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

                NativeApi.MessageBox(IntPtr.Zero, message.ToString(), $"{AppName}: Aborted!",
                    (long) (MessageBoxKind.IconStop | MessageBoxKind.SystemModal));

                result = exception.HResult;
            }
            finally
            {
                var service = launcher?.GetService();
                
                service?.SendEmptyMessage();
                ctx.Cancel();
            }

            return result;
        }
    }
}