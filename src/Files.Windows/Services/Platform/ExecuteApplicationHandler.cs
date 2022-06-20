using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Files.Services.Platform.Interfaces;
using Material.Dialog;
using Material.Dialog.Enums;

namespace Files.Windows.Services.Platform
{
    public class ExecuteApplicationHandler : IPlatformSupportExecuteApplication
    {
        public void NativeExecuteApplication(string path)
        {
            throw new System.NotImplementedException();
        }

        public bool IsExecutableApplication(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".exe" => true,
                ".com" => true,
                _ => false
            };
        }
        
        internal static void InvokeNewProcess(string execPath, string? args)
        {
            Task.Factory.StartNew(delegate
            {
                var param = new ProcessStartInfo
                {
                    FileName = execPath,
                    Arguments = args ?? string.Empty,
                    WorkingDirectory = Path.GetDirectoryName(execPath)!,
                };
                Process.Start(param);
            }).ContinueWith(delegate(Task task)
            {
                if (!task.IsFaulted) 
                    return;
                
                var exception = task.Exception?.InnerException ?? new NullReferenceException("Unknown exception");
                var builder = new StringBuilder();

                builder.AppendLine("Cannot start process:");
                builder.AppendLine(exception.Message);

                Dispatcher.UIThread.Post(delegate
                {
                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        SupportingText = builder.ToString(),
                        DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok),
                        Borderless = false,
                        ContentHeader = "Error",
                        WindowTitle = "Error"
                    });
                    dialog.Show();
                });
            });
        }
    }
}