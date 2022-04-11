using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Files.Collections;

namespace Files.Wrappers
{
    public class ProcessWrapper : IDisposable
    {
        public enum MessageType
        {
            Output,
            Error
        }
        
        public class Message
        {
            public string? Text;
            public MessageType Type;

            public override string ToString() => $"{(Type == MessageType.Error ? "E" : "O")}:{Text}";
        }
        
        private Process process;
        private ObservableQueue<Message?> outputs;
        private ManualResetEventSlim outputEvent;

        public ProcessWrapper(string execPath, string? workDir = null, string? args = null)
        {
            outputEvent = new ManualResetEventSlim();
            
            outputs = new ObservableQueue<Message?>();
            outputs.CollectionChanged += OnOutputCollectionChanged;
            
            var startInfo = new ProcessStartInfo
            {
                FileName = execPath, UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8,
                WorkingDirectory = workDir ?? Path.GetDirectoryName(execPath),
                Arguments = args ?? string.Empty
            };
            
            var _process = new Process
            {
                StartInfo = startInfo
            };
            
            _process.Exited += OnProcessExited;
            _process.ErrorDataReceived += OnErrorDataReceived;
            _process.OutputDataReceived += OnOutputDataReceived;
            
            process = _process;
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            if(!outputEvent.IsSet)
                outputEvent.Set();
        }

        private void OnOutputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(!outputEvent.IsSet)
                outputEvent.Set();
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            
            outputs.Enqueue(new Message
                {Text = e.Data, Type = MessageType.Error});
        }
        
        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            
            outputs.Enqueue(new Message
                {Text = e.Data, Type = MessageType.Output});
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public async IAsyncEnumerable<Message> Start()
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.StandardInput.WriteLineAsync();
            
            bool Next(out Message? msg)
            {
                msg = default;

                if (outputs.Count != 0)
                    return outputs.TryDequeue(out msg);
                
                if (process.HasExited)
                    return false;
                    
                outputEvent.Wait(1000);
                outputEvent.Reset();

                return outputs.TryDequeue(out msg);
            }

            while (Next(out var msg) || !process.HasExited)
            {
                if(msg == null)
                    continue;
                
                yield return msg;
            }
            
            process.CancelOutputRead();
            process.CancelErrorRead();
        }

        public void Kill() => process.Kill();
        
        public void Dispose()
        {
            if(!process.HasExited)
                process.Kill();
            
            process.Exited -= OnProcessExited;
            process.ErrorDataReceived -= OnErrorDataReceived;
            process.OutputDataReceived -= OnOutputDataReceived;
            
            process.Close();
            process.Dispose();
            
            outputEvent.Dispose();
        }

        public static async IAsyncEnumerable<Message> StartProcessAndCatch(string execPath, string? workDir = null,
            string? args = null)
        {
            using (var wrapper = new ProcessWrapper(execPath, workDir, args))
            {
                await foreach (var msg in wrapper.Start())
                {
                    yield return msg;
                }
            }
        }
    }
}