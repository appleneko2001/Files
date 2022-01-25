using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Files.Models.Devices;
using Files.Services.Platform;
using Files.Services.Platform.Interfaces;
using Files.Windows.Models;
using Files.Windows.Services.Native;
using Files.Windows.Services.Native.Enums;
using Material.Dialog;
using Material.Dialog.Enums;

namespace Files.Windows.Services
{
    public class WindowsApiBridge : PlatformSpecificBridge
    {
        private Collection<DeviceModel> _devices;
        private MyComputerDeviceModel _myComputerModel;
        public WindowsApiBridge()
        {
            _myComputerModel = new MyComputerDeviceModel();
            
            _devices = new Collection<DeviceModel>
            {
                _myComputerModel
            };
        }
        
        public override void PopupMessageWindow(string title, string content)
        {
            NativeApi.MessageBox(IntPtr.Zero, content, title, (long) MessageBoxKind.Default);
        }

        /// <summary>
        /// Get all storage entries, including removable storage.
        /// </summary>
        /// <returns>Return all available storages.</returns>
        public override IReadOnlyCollection<DeviceModel> GetDeviceEntries()
        {
            return _devices;
        }

        public override void NativeExecuteApplication(string execPath)
        {
            Task.Factory.StartNew(delegate
            {
                var param = new ProcessStartInfo
                {
                    FileName = execPath,
                    WorkingDirectory = Path.GetDirectoryName(execPath)!,
                };
                Process.Start(param);
            }).ContinueWith(delegate(Task task)
            {
                if (!task.IsFaulted) 
                    return;
                
                var exception = task.Exception.InnerException;
                var builder = new StringBuilder();

                builder.AppendLine("Cannot start process:");
                builder.AppendLine(task.Exception.InnerException.Message);

                Dispatcher.UIThread.Post(async delegate
                {
                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                    {
                        SupportingText = builder.ToString(),
                        DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok),
                        Borderless = false,
                        ContentHeader = "Error",
                        WindowTitle = "Error"
                    });
                    await dialog.Show();
                });
            });
        }

        public override bool IsExecutableApplication(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".exe" => true,
                ".com" => true,
                _ => false
            };
        }

        public override void LetPlatformHandleThisFile(string path)
        {
            OpenFileProcedure(delegate
            {
                var param = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "open",
                    WorkingDirectory = Path.GetDirectoryName(path)!
                };
                Process.Start(param);
            }, $"The file \"{path}\" cannot be opened or executed: ", path);
        }

        public override void ShowOpenWithApplicationDialog(string path)
        {
            ShowOpenWithDialog(path);
        }

        private static void OpenFileProcedure(Action del, string errorMsg, string path)
        {
            Task.Factory.StartNew(del).ContinueWith(delegate(Task task)
            {
                if (task.IsFaulted)
                {
                    var exception = task.Exception.InnerException;

                    if (exception is Win32Exception e)
                    {
                        // 1155 - No application is associated with the specified file for this operation.
                        if (e.NativeErrorCode == 1155)
                        {
                            if (path != null)
                            {
                                ShowOpenWithDialog(path);
                                return;
                            }
                        }
                    }
                    
                    var builder = new StringBuilder();

                    builder.AppendLine(errorMsg);
                    builder.AppendLine(task.Exception.InnerException.Message);

                    Dispatcher.UIThread.Post(async delegate
                    {
                        var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                        {
                            SupportingText = builder.ToString(),
                            DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok),
                            Borderless = false,
                            ContentHeader = "Error",
                            WindowTitle = "Error",
                        });
                        await dialog.Show();
                    });
                }
            });
        }
        
        /// <summary>
        /// Show "open file with" dialog. Feature is supported on Windows OS only, not sure how to implement in Unix.
        /// </summary>
        /// <param name="path">The file location.</param>
        private static void ShowOpenWithDialog(string path) {
            // Works only in Windows OS
            OpenFileProcedure(delegate
            {
                var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
                args += ",OpenAs_RunDLL " + path;
                Process.Start("rundll32.exe", args);
            }, $"The file \"{path}\" cannot be opened or executed: ", path);
        }
    }
}