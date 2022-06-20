using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Files.Models.Devices;
using Files.Services.Platform;
using Files.Services.Platform.Interfaces;
using Files.Windows.Models;
using Files.Windows.Services.Native;
using Files.Windows.Services.Native.Enums;
using Files.Windows.Services.Platform;
using Material.Dialog;
using Material.Dialog.Enums;

// ReSharper disable InconsistentNaming

namespace Files.Windows.Services
{
    public class WindowsApiBridge : IPlatformSupportNativeExplorer,
        IPlatformSupportGetIcon,
        IPlatformSupportOpenFilePrimaryAction,
        IPlatformSupportShowMessage,
        IPlatformSupportDeviceEntries
    {
        private const string _nativeExplorerName = "Windows Explorer";
        public string NativeExplorerName => _nativeExplorerName;
        
        private readonly Collection<DeviceModel> _devices;

        public WindowsApiBridge()
        {
            var myComputerModel = new MyComputerDeviceModel();
            
            _devices = new Collection<DeviceModel>
            {
                myComputerModel
            };
        }
        
        public void PopupMessageWindow(string title, string content)
        {
            NativeApi.MessageBox(IntPtr.Zero, content, title, (long) MessageBoxKind.Default);
        }

        /// <summary>
        /// Get all storage entries, including removable storage.
        /// </summary>
        /// <returns>Return all available storages.</returns>
        public IReadOnlyCollection<DeviceModel> GetDeviceEntries()
        {
            return _devices;
        }

        public void NativeExecuteApplication(string execPath)
        {
            ExecuteApplicationHandler.InvokeNewProcess(execPath, null);
        }

        public void LetPlatformHandleThisFile(string path)
        {
            Task.Factory.StartNew(OpenFilePrimaryActionProcedure, path)
                .ContinueWith(PostOpenFileProcedureTask, path);
        }

        



        private static void OpenFilePrimaryActionProcedure(object? arg)
        {
            if (arg is not string path)
                return;
            
            var param = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open",
                WorkingDirectory = Path.GetDirectoryName(path)!
            };
            Process.Start(param);
        }

        internal static void OpenFileWithProgramProcedure(object? arg)
        {
            if (arg is not string path)
                return;

            var shellPath = Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), 
                    "shell32.dll");
            
            var builder = new StringBuilder(shellPath);
            
            builder.Append(",OpenAs_RunDLL ");
            builder.Append(path);
            
            Process.Start("rundll32.exe", builder.ToString());
        }

        internal static void PostOpenFileProcedureTask(Task task, object? arg)
        {
            if (!task.IsFaulted)
                return;

            if (arg is not string path)
                return;
                
            var exception = task.Exception?.InnerException;

            if (exception is Win32Exception e)
            {
                // 1155 - No application is associated with the specified file for this operation.
                // ReSharper disable once MergeIntoPattern
                if (e.NativeErrorCode == 1155)
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        Task.Factory.StartNew(OpenFileWithProgramProcedure, path)
                            .ContinueWith(PostOpenFileProcedureTask, path);
                        return;
                    }
                }
            }
                    
            var builder = new StringBuilder();

            builder.AppendLine($"The file \"{path}\" cannot be opened or executed: ");
            builder.AppendLine(exception?.Message);

            var message = builder.ToString();

            Dispatcher.UIThread.InvokeAsync(delegate
            {
                var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams
                {
                    SupportingText = message,
                    DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok),
                    Borderless = false,
                    ContentHeader = "Error",
                    WindowTitle = "Error"
                });
                dialog.Show();
            });
        }

        public void OpenFolderWithNativeExplorer(string path)
        {
            ExecuteApplicationHandler.InvokeNewProcess("explorer.exe", path);
        }

        public bool CanOpenFolderWithNativeExplorer(string path)
        {
            return true;
        }

        public bool CanGetIconForFile(string path)
        {
            // TODO: Support shortcut file (.lnk)
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".exe" => true,
                _ => false
            };
        }

        /// <summary>
        /// Used for extract icon by parsing executable file 
        /// </summary>
        public NativeResourcePointer GetIconForFile(string path, int w, int h)
        {
            /*const int DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
            const int LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;
            const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

            const int flags = DONT_RESOLVE_DLL_REFERENCES | LOAD_LIBRARY_AS_DATAFILE | LOAD_IGNORE_CODE_AUTHZ_LEVEL;

            var hModule = new Win32NativeLibPointer(NativeApi.LoadLibraryEx(path, IntPtr.Zero, flags));*/

            return new Win32NativeIconPointer(IntPtr.Zero);
        }

        public Stream? GetIconStreamForFile(string path, int w, int h)
        {
            var hIcon = NativeApi.ExtractIcon(
                Marshal.GetHINSTANCE(GetType().Module), path, 0);

            if (hIcon == IntPtr.Zero)
                return null;
            
            using var icon = Icon.FromHandle(hIcon);

            var memoryStream = new MemoryStream();
            
            icon.Save(memoryStream);

            return memoryStream;
        }

        public bool CanHandleThisFile(string path)
        {
            return Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".exe" => false,
                ".com" => false,
                _ => true
            };
        }
    }
}