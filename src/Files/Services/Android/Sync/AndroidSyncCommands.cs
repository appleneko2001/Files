using System.Collections.Generic;
using Files.Adb.Commands.Sync;
using Files.Adb.Extensions;
using Files.Adb.Models;
using Files.Models.Android.Storages;

namespace Files.Services.Android.Sync
{
    public class AndroidSyncCommands
    {
        private readonly AndroidDebugBackend _backend;
        
        public AndroidSyncCommands(AndroidDebugBackend backend)
        {
            _backend = backend;
        }
        
        public async IAsyncEnumerable<AdbListFilesItemModel?> GetListFilesViaSyncAsync(AdbConnection conn, string path)
        {
            using var adbStream = _backend.GetAdbStream();
            
            if (adbStream == null)
                yield break;

            await foreach (var item in adbStream
                               .SetDevice(conn)
                               .UseSyncFeature()
                               .UseCommand(new AdbGetFilesListCommand())
                               .WithParameter("remotePath", path)
                               .Execute())
            {
                if (item == null)
                    continue;

                if (item.IsFailed)
                    throw item.Exception;
                
                if(item.IsSuccess)
                    yield break;

                var model = new AdbListFilesItemModel();
                model.Apply(item.Result!);

                yield return model;
            }
        }
        
        public async IAsyncEnumerable<string> WalkDirViaSyncAsync(AdbConnection conn, string path, bool onlyFile = true)
        {
            using var adbStream = _backend.GetAdbStream();
            
            if (adbStream == null)
                yield break;

            await foreach (var item in adbStream
                               .SetDevice(conn)
                               .UseSyncFeature()
                               .UseCommand(new AdbGetFilesListCommand())
                               .WithParameter("remotePath", path)
                               .Execute())
            {
                if (item == null)
                    continue;

                if (item.IsFailed)
                    throw item.Exception;
                
                if(item.IsSuccess)
                    yield break;

                var model = item.Result!;

                if (model.IsDirectory)
                {
                    if (!onlyFile)
                        yield return model.Path;
                    
                    await foreach (var i in WalkDirViaSyncAsync(conn, model.Path, onlyFile))
                        yield return i;

                    continue;
                }

                yield return model.Path;
            }
        }
    }
}