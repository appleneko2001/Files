using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Avalonia.Input;
using Files.Adb;
using Files.Adb.Extensions;
using Files.Adb.Models.Connections;
using Files.Adb.Models.Sync;
using Files.Adb.Operations;
using Files.Models.Android.Storages;
using Files.ViewModels;
using Files.ViewModels.Browser.Files.Android;
using Files.ViewModels.Context.Menus;
using Material.Icons;
using AdbDeviceModel = Files.Adb.Models.AdbDeviceModel;
using ContextMenuItem = Files.ViewModels.Context.Menus.ContextMenuItemViewModel;

namespace Files.Services.Android
{
    public class AndroidDebugBackend
    {
        private static AndroidDebugBackend _instance;

        public static AndroidDebugBackend Instance =>
            _instance ?? throw new NullReferenceException("AndroidDebugBackend is not initialized.");
        
        public static void Initiate(FilesApp app)
        {
            if(_instance != null)
                throw new InvalidOperationException("AndroidDebugBackend is already initialized.");
            
            _instance = new AndroidDebugBackend(app);
        }
        
        private readonly string? _adbPath;

        public string AdbPath =>
            _adbPath ?? throw new ArgumentNullException(nameof(AdbPath),
                "Adb executable path is not set. You can set it in environment variable ADB_EXECUTABLE.");

        private AdbClient? _client;
        
        private event EventHandler? _updateDevicesEvent;
        public event EventHandler UpdateDevicesEvent
        {
            add => _updateDevicesEvent += value;
            remove => _updateDevicesEvent -= value;
        }
        
        private AndroidDebugBackend(FilesApp app)
        {
            app.ApplicationInitializationCompleted+= OnApplicationInitializationCompleted;

            _adbPath = Environment.GetEnvironmentVariable("ADB_EXECUTABLE");
        }

        private void OnApplicationInitializationCompleted(object sender, EventArgs e)
        {
            if(sender is not FilesApp app)
                return;
            
            app.ApplicationInitializationCompleted -= OnApplicationInitializationCompleted;

            // Default adb daemon port
            const int port = 5037;
            
            _client = new AdbClient(port: port)
            {
                Encoding = Encoding.UTF8
            };

            if (!_client.IsRunning())
            {
                // ADB is not running. Try to start it.
                if (_adbPath == null)
                {
                    Console.WriteLine("ADB executable path is not set. You can set it in environment variable ADB_EXECUTABLE.");
                    return;
                }

                Console.WriteLine("Starting ADB daemon...");
                Process.Start(_adbPath, $"-P {port} start-server");
            }

            ContextMenuBackend
                .RegisterContextMenuItems<AdbFileSystemItemViewModel>(InitiateFolderContextMenu());
            ContextMenuBackend
                .RegisterContextMenuItems<AdbFileSystemItemViewModel>(InitiateContextMenuForAdbService());
            
            app.ApplicationShutdown += OnApplicationShutdown;
            
            AppBackend.Instance.DeviceCollectionChanged += OnDeviceCollectionChanged;
        }
        
        private IEnumerable<ContextMenuItemViewModelBase> InitiateFolderContextMenu()
        {
            var commands = CommandsBackend.Instance;
            
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItem("Open folder", keyGesture: KeyGesture.Parse("Enter"), command: commands.OpenFolderInCurrentViewCommand),
                //new ContextMenuItemViewModel("Open folder in new tab"),
                new ContextMenuItem("Open folder in new window", command: commands.OpenFolderInNewWindowCommand)
            };

            return list;
        }

        private IEnumerable<ContextMenuItemViewModelBase> InitiateContextMenuForAdbService()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new ContextMenuItem("Pull", new MaterialIconViewModel(MaterialIconKind.Android))
            };

            return list;
        }

        private void OnDeviceCollectionChanged(object sender, EventArgs e)
        {
            // Update device list
            _updateDevicesEvent?.Invoke(this, EventArgs.Empty);
        }

        public async IAsyncEnumerable<AdbDeviceModel> GetDevicesAsync()
        {
            using (var adbStream = _client?.CreateStream())
            {
                if(adbStream == null)
                    yield break;

                foreach (var model in adbStream
                             .GetDevices()
                             .ToDevicesList())
                {
                    yield return model;
                }
            }
        }

        public async IAsyncEnumerable<KeyValuePair<string, string>> GetPropertiesAsync(IAdbConnection conn,
            string? startWith = null)
        {
            bool TryParseLine(string line, out KeyValuePair<string, string>? result)
            {
                using (var reader = new StringReader(line))
                {
                    var mode = 0;
                    
                    var catchBlock = false;
                    
                    var builder = new StringBuilder();
                    
                    string Flush()
                    {
                        var result = builder.ToString();
                        builder.Clear();
                        return result;
                    }
                    
                    string key = string.Empty, value = string.Empty;
                    
                    bool GetNextChar(out char? c)
                    {
                        c = null;
                    
                        var read = reader!.Read();
                        if (read == -1)
                            return false;

                        c = (char) read;
                        return true;
                    }

                    while (GetNextChar(out var c))
                    {
                        switch (c)
                        {
                            case '[':
                                catchBlock = true;
                                continue;
                                
                            case ']':
                                catchBlock = false;
                                switch (mode)
                                {
                                    case 0:
                                        key = Flush();
                                        break;
                                    
                                    case 1:
                                        value = Flush();
                                        break; 
                                }
                                mode = (mode + 1) % 2;
                                continue;
                        }

                        if(catchBlock)
                            builder.Append(c);
                    }

                    if (key == string.Empty || value == string.Empty)
                    {
                        result = null;
                        return false;
                    }

                    result = new KeyValuePair<string, string>(key, value);
                    return true;
                }
            }

            KeyValuePair<string, string>? Parser(string l)
            {
                if (l.ToLowerInvariant().StartsWith("list of properties"))
                    return null;
                
                return TryParseLine(l, out var result) ? result : null;
            }
            
            if(_client == null)
                yield break;

            // TODO: Pass serial number or ip address

            using (var adbStream = _client.CreateStream())
            {
                foreach (var prop in adbStream
                             .SetDevice(conn)
                             .Shell("getprop")
                             .ProcessAndToList(Parser, true))
                {
                    if(!prop.HasValue)
                        continue;

                    yield return prop.Value;
                }
            }
        }

        public async IAsyncEnumerable<AdbListFilesItemModel?> GetListFilesAsync(IAdbConnection conn, string path)
        {
            // -l long list
            // -a show hidden files
            // -p put a '/' at the end of each entry
            
            // Last line will be exit code with header !!

            //"ls -lpaF /sdcard/; echo !!$?"

            var args = $"ls -lpa \"{path}\"; echo !!$?";
            
            var lastLine = string.Empty;

            AdbListFilesItemModel? Parser(string l)
            {
                var test = l.ToLowerInvariant();
                
                // Contain exit code of ls command
                if (test.StartsWith("!!"))
                {
                    // ls unable to list directory
                    if (test.Remove(0, 2) == "1")
                        throw new UnauthorizedAccessException(lastLine);
                    
                    return null;
                }

                if (test.StartsWith("total"))
                    return null;

                if (test.StartsWith("/system/bin/sh") || test.StartsWith("ls"))
                {
                    lastLine = l;
                    return null;//throw new FormatException(test);
                }

                var model = new AdbListFilesItemModel();
                model.Parse(l.Trim().Replace("\\", ""), path);

                return model;
            }

            using (var adbStream = _client?.CreateStream())
            {
                if(adbStream == null)
                    yield break;

                foreach (var model in adbStream
                             .SetDevice(conn)
                             .Shell(args)
                             .ProcessAndToList(Parser, true))
                {
                    if (model == null)
                        continue;

                    yield return model;
                }
            }
        }
        
        // TODO: not working
        public async IAsyncEnumerable<AdbListFilesItemModel?> GetListFilesViaSyncAsync(IAdbConnection conn, string path)
        {
            using (var adbStream = _client?.CreateStream())
            {
                if(adbStream == null)
                    yield break;

                await foreach (var item in adbStream
                                   .SetDevice(conn)
                                   .UseSyncFeature()
                                   .UseOperation(new AdbGetFilesListOperation())
                                   .WithParameter("remotePath", path)
                                   .PerformOperation())
                {
                    if (item == null)
                        continue;

                    if (item.Result is Exception e)
                        throw e;

                    if (item.Result is not AdbFileEntry entry)
                        continue;
                    
                    var model = new AdbListFilesItemModel();
                    model.Apply(entry);
                        
                    yield return new AdbListFilesItemModel();
                }
            }
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            if(sender is not FilesApp app)
                return;
            
            app.ApplicationShutdown -= OnApplicationShutdown;
            AppBackend.Instance.DeviceCollectionChanged -= OnDeviceCollectionChanged;
        }
    }
}