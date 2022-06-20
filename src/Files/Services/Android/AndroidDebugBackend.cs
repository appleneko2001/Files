using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Files.Adb;
using Files.Adb.Extensions;
using Files.Adb.Models;
using Files.Models.Android.Storages;
using Files.Services.Android.Actions;
using Files.Services.Android.Sync;
using Files.ViewModels.Context.Menus;
using ContextMenuItem = Files.ViewModels.Context.Menus.ContextMenuItemViewModel;

namespace Files.Services.Android
{
    public class AndroidDebugBackend
    {
        private static AndroidDebugBackend? _instance;

        public static AndroidDebugBackend Instance =>
            _instance ?? throw new NullReferenceException("AndroidDebugBackend is not initialized.");

        public static void Initiate(FilesApp app)
        {
            if (_instance != null)
                throw new InvalidOperationException("AndroidDebugBackend is already initialized.");

            _instance = new AndroidDebugBackend(app);
        }

        public string AdbPath =>
            _adbPath ?? throw new ArgumentNullException(nameof(AdbPath),
                "Adb executable path is not set. You can set it in environment variable ADB_EXECUTABLE.");

        public IEnumerable<KeyValuePair<string, string>> Devices => _devices;

        public AndroidSyncCommands? SyncBackend => _syncBackend;

        public event NotifyCollectionChangedEventHandler UpdateDevicesEvent
        {
            add => _devices.CollectionChanged += value;
            remove => _devices.CollectionChanged -= value;
        }

        private readonly string? _adbPath;

        private readonly ObservableCollection<KeyValuePair<string, string>> _devices;
        private readonly CancellationTokenSource _shutdownCancellationTokenSource;
        private AdbClient? _client;
        private AndroidSyncCommands? _syncBackend;
        private const int DefaultPort = 5037;

        private AndroidDebugBackend(FilesApp app)
        {
            _syncBackend = new AndroidSyncCommands(this);
            
            _shutdownCancellationTokenSource = new CancellationTokenSource();
            
            _devices = new ObservableCollection<KeyValuePair<string, string>>();
            app.ApplicationInitializationCompleted += OnApplicationInitializationCompleted;

            _adbPath = Environment.GetEnvironmentVariable("ADB_EXECUTABLE");
        }

        private void OnApplicationInitializationCompleted(object sender, EventArgs e)
        {
            if (sender is not FilesApp app)
                return;

            app.ApplicationInitializationCompleted -= OnApplicationInitializationCompleted;

            // Default adb daemon port

            _client = new AdbClient(port: DefaultPort)
            {
                Encoding = Encoding.UTF8
            };

            InitTrackDevicesTask(_shutdownCancellationTokenSource.Token);

            //ContextMenuBackend
            //    .RegisterContextMenuItems(InitiateFolderContextMenu());
            ContextMenuBackend
                .RegisterContextMenuItems(InitiateContextMenuForAdbService());

            app.ApplicationShutdown += OnApplicationShutdown;
        }

        public async IAsyncEnumerable<string> GetDevicesAsync()
        {
            using var adbStream = _client?.CreateStream();

            if (adbStream == null)
                yield break;
            
            await foreach (var model in adbStream
                         .GetDevices()
                         .ToEnumerableAsync())
                yield return model;
        }

        public IEnumerable<KeyValuePair<string, string>> GetProperties(AdbConnection conn,
            string? startWith = null)
        {
            bool TryParseLine(string line, out KeyValuePair<string, string>? result)
            {
                using var reader = new StringReader(line);

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

                    var read = reader.Read();
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

                    if (catchBlock)
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

            KeyValuePair<string, string>? Parser(string l)
            {
                if (l.ToLowerInvariant().StartsWith("list of properties"))
                    return null;

                return TryParseLine(l, out var result) ? result : null;
            }

            if (_client == null)
                yield break;

            using var adbStream = _client.CreateStream();

            foreach (var prop in adbStream
                         .SetDevice(conn)
                         .Shell("getprop")
                         .ProcessAndToList(Parser, true))
            {
                if (!prop.HasValue)
                    continue;

                yield return prop.Value;
            }
        }

        public async IAsyncEnumerable<AdbListFilesItemModel?> GetListFilesAsync(AdbConnection conn, string path)
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
                    return null; //throw new FormatException(test);
                }

                var model = new AdbListFilesItemModel();
                model.Parse(l.Trim().Replace("\\", ""), path);

                return model;
            }

            using var adbStream = _client?.CreateStream();
            if (adbStream == null)
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

        public async IAsyncEnumerable<string> WalkDirectoryTreeAsync(AdbConnection conn, string path)
        {
            using var adbStream = _client?.CreateStream();
            
            // ReSharper disable once UseNullPropagation
            if(Equals(adbStream, null))
                yield break;

            await foreach (var element in adbStream
                               .SetDevice(conn)
                               .Execute($"find \"{path}\" -type f")
                               .ToEnumerableAsync(true))
                yield return element;
        }

        public Stream? GetFileStreamViaShell(AdbConnection conn, string file)
        {
            var adbStream = _client?.CreateStream();
            
            // ReSharper disable once UseNullPropagation
            if(Equals(adbStream, null))
                return null;

            //var guid = Guid.NewGuid().ToString("N");
            //var inject = "; !!{guid}_result_code=$?";

            // TODO: Know result code after run and remove inject code while reading
            return adbStream
                .SetDevice(conn)
                .Execute($"cat \"{file}\"")
                .GetReadOnlyStream();
        }

        public AdbStream? GetAdbStream() => _client?.CreateStream();

        private IReadOnlyList<ContextMenuItemViewModelBase> InitiateContextMenuForAdbService()
        {
            var list = new List<ContextMenuItemViewModelBase>
            {
                new PullAdbContextMenuItemAction()
            };

            return list;
        }

        private void InitTrackDevicesTask(CancellationToken cancellationToken) =>
            Task.Factory.StartNew(TrackDevicesTask, cancellationToken)
                .ContinueWith(PostTrackDevicesTask, cancellationToken);

        private void TrackDevicesTask()
        {
            if (_client == null)
                throw new Exception("!ADB Client instance is null");

            if (!_client!.IsRunning())
            {
                // ADB is not running. Try to start it.
                if (_adbPath == null)
                {
                    const string message = "ADB executable path is not set. " +
                                           "You can set it in environment variable ADB_EXECUTABLE.";
                    Console.WriteLine(message);
                    throw new Exception($"!{message}");
                }

                Console.WriteLine("Starting ADB daemon...");
                var daemon = Process.Start(_adbPath, $"-P {DefaultPort} start-server");

                if (daemon == null)
                    throw new NullReferenceException("!Process instance is null");

                daemon.WaitForExit();
            }

            using var connection = _client.CreateStream();

            // TODO: With update status
            
            foreach (var data in connection
                         .TrackDevices()
                         .ReceiveLinesContinuous())
            {
                using var reader = new StringReader(data);

                // ReSharper disable once MoveVariableDeclarationInsideLoopCondition
                string? line;

                var list = new List<string>();

                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split('\t');

                    list.Add(split[0]);
                    //var status = split[1];
                }

                foreach (var devices in list
                             .Select(d => _devices.Where(a => a.Key != d)))
                {
                    foreach(var device in devices)
                        _devices.Add(device);
                }

                foreach (var device in _devices)
                {
                    if(list.Contains(device.Key))
                        continue;

                    _devices.Remove(device);
                }
            }
        }

        private void PostTrackDevicesTask(Task task)
        {
            if (task.IsCanceled)
            {
                task.Dispose();
                _shutdownCancellationTokenSource.Dispose();
                return;
            }

            // Connection is interrupted
            if (task.IsFaulted)
            {
                var exception = task.Exception!.InnerException;
                if (exception!.Message.StartsWith('!'))
                {
                    task.Dispose();
                    _shutdownCancellationTokenSource.Dispose();
                    return;
                }
            }

            InitTrackDevicesTask(_shutdownCancellationTokenSource.Token);
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            if (sender is not FilesApp app)
                return;

            _shutdownCancellationTokenSource.Cancel();

            app.ApplicationShutdown -= OnApplicationShutdown;
        }
    }
}