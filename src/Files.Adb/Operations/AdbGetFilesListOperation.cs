using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Files.Adb.Models.Sync;
using Files.Adb.Operations.Results;

namespace Files.Adb.Operations
{
    public class AdbGetFilesListOperation : AdbOperationBase, IAdbOperation
    {
        public const string Header = "LIST";

        /// <summary>
        /// <p>Initializes a new instance of the <see cref="AdbGetFilesListOperation"/> class.</p>
        /// Parameters:
        /// <list type="bullet">
        /// <item>remotePath - path to the remote directory</item>
        /// </list>
        /// </summary>
        public AdbGetFilesListOperation()
        {
            
        }

        public async IAsyncEnumerable<AdbOperationResult> RunAsync(IDictionary<string, object> args,
            CancellationToken cancellationToken)
        {
            if (AdbStream == null)
                throw new ArgumentNullException(nameof(AdbStream));
            
            var remotePath = (string)args["remotePath"];

            await AdbStream.SendSyncRequestAsync(Header, remotePath);

            await using (var stream = AdbStream.GetStream())
            {
                var header = new byte[4];
                
                while (await stream.ReadAsync(header) == 4)
                {
                    var line = Encoding.ASCII.GetString(header);

                    switch (line)
                    {
                        case Fail:
                            yield return new AdbOperationResult
                            {
                                Progress = 0,
                                Result = new Exception("Unable to perform operation")
                            };
                            yield break;
                        case Dent:
                            yield return new AdbOperationResult
                            {
                                Progress = 0,
                                Result = new Exception("Unknown response")
                            };
                            yield break;
                    }

                    if(line == Done)
                    {
                        break;
                    }

                    var stat = new byte[12];
                    await stream.ReadAsync(stat);

                    var lenBuf = new byte[4];
                    await stream.ReadAsync(lenBuf);

                    var lenStr = Encoding.ASCII.GetString(lenBuf);
                    var len = int.Parse(lenStr, NumberStyles.HexNumber);
                    var pathBuf = new byte[len];
                    await stream.ReadAsync(pathBuf);

                    var path = Encoding.UTF8.GetString(pathBuf);

                    var adbFileEntry = new AdbFileEntry(stat, path);

                    yield return new AdbOperationResult
                    {
                        Progress = 0,
                        Result = adbFileEntry
                    };
                }
            }
        }
    }
}