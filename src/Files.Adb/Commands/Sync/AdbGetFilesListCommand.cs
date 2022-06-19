using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Files.Adb.Models.Sync;

namespace Files.Adb.Commands.Sync
{
    public class AdbGetFilesListCommand : AdbSyncCommandBase, IAdbSyncCommand<AdbSyncListFilesResult>
    {
        public const string Header = "LIST";

        public static readonly string[] IgnoredNames = { ".", ".." };

        /// <summary>
        /// <i>Version: v1</i>
        /// <p>Initializes a new instance of the <see cref="AdbGetFilesListCommand"/> class.</p>
        /// Parameters:
        /// <list type="bullet">
        /// <item>remotePath - path to the remote directory</item>
        /// </list>
        /// </summary>
        public AdbGetFilesListCommand()
        {
            
        }

        public async IAsyncEnumerable<AdbSyncListFilesResult> RunAsync(IDictionary<string, object> args,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (AdbStream == null)
                throw new ArgumentNullException(nameof(AdbStream));
            
            var remotePath = (string)args["remotePath"];

            await AdbStream.SendSyncRequestAsync(Header, remotePath);

            await using var stream = AdbStream.GetStream();

            var encoding = Encoding.UTF8;

            using var reader = new BinaryReader(stream, encoding);
            
            var responseData = new byte[4];
            while (await stream.ReadAsync(responseData, cancellationToken) == 4)
            {
                var response = Encoding.ASCII.GetString(responseData);
                switch (response)
                {
                    case Fail:
                        var reasonStrLen = reader.ReadInt32();
                        var reason = encoding.GetString(reader.ReadBytes(reasonStrLen));
                            
                        yield return new AdbSyncListFilesResult
                        {
                            Progress = 0,
                            IsSuccess = false,
                            IsFailed = true,
                            Exception = new Exception("Unable to perform operation. "
                                                      + reason.Replace('\n', ' '))
                        };
                        yield break;
                    
                    // Receive single entry information
                    case Dent:
                        var mode = reader.ReadInt32();
                        var size = reader.ReadInt32();
                        var modTime = reader.ReadInt32();
                        var nameLength = reader.ReadInt32();
                        var name = encoding.GetString(reader.ReadBytes(nameLength));
                        
                        // device might dent some folder like "." and ".." they shouldn't be appeared here.
                        // we need skip those entry and go next cycle.
                        if(IsBlacklistedName(name))
                            continue;

                        var fullName = Path.Combine(remotePath, name);

                        yield return new AdbSyncListFilesResult
                        {
                            Progress = 0,
                            Result = new AdbFileEntry(mode, size, modTime, fullName)
                        };
                        continue;
                    
                    case Done:
                        yield return new AdbSyncListFilesResult
                        {
                            Progress = 0,
                            IsSuccess = true
                        };
                        yield break;
                }
            }
        }

        IAsyncEnumerable<AdbSyncCommandResultBase> 
            IAdbSyncCommand.RunAsync(IDictionary<string, object> args, CancellationToken cancellationToken)
        {
            return RunAsync(args, cancellationToken);
        }

        private static bool IsBlacklistedName(string name)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < IgnoredNames.Length; index++)
            {
                var item = IgnoredNames[index];
                if (item.Equals(name))
                    return true;
            }

            return false;
        } 
    }
}