using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Files.Collections;
using Files.Tasks;
using Files.Tasks.Schedules;
using Files.Views.Models.Browser.Preview;

namespace Files.Services
{
    public class PreviewManagerBackend
    {
        private static Thread? _enumerateQueueThread;
        private static ObservableQueue<TaskScheduleModel> _queues;
        private static ManualResetEventSlim _manualResetEvent;

        static PreviewManagerBackend()
        {
            _manualResetEvent = new ManualResetEventSlim();
            
            _queues = new ObservableQueue<TaskScheduleModel>();
            _queues.CollectionChanged += OnCollectionChanged;
        }

        private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            if (_enumerateQueueThread != null)
            {
                if(!_manualResetEvent.IsSet)
                    _manualResetEvent.Set();
                return;
            }

            _enumerateQueueThread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    while (true)
                    {
                        if (_queues.Count == decimal.Zero)
                        {
                            _manualResetEvent.Wait();
                            _manualResetEvent.Reset();
                            continue;
                        }

                        try
                        {
                            using (var model = _queues.Dequeue())
                            {
                                if (!model.CancellationToken.IsCancellationRequested)
                                {
                                    var task = model.Task;
                                    task.Start();
                                    task.Wait();
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Preview management thread has stopped due exception thrown: {e.Message}");
                    Console.WriteLine($"Stacktrace: {e.StackTrace}");

                    _enumerateQueueThread = null;
                }
            }));
            _enumerateQueueThread.Name = "Preview queue thread";
            _enumerateQueueThread.Start();
        }

        public static void GetLazyPreview(FileInfo fi, Action<PreviewableViewModelBase> OnComplete = null,
            CancellationToken _cancellationToken = default)
        {
            /*
            var scheduleTask = new Task(GetPreviewCore, model, _cancellationToken);
            scheduleTask.ContinueWith(delegate(Task task)
            {
                if(OnFinalizeGetPreviewTask(task, model))
                    OnComplete?.Invoke(model.Result!);
            }, _cancellationToken);
            
            _queues?.Enqueue(scheduleTask);*/
            var model = new GetPreviewLocalFileTaskModel
            {
                LocalFileInfo = fi
            };
            
            /*var result = new LazyPicturePreviewableViewModelBase(delegate
            {
                return fi.OpenRead();
            }, delegate(Stream stream)
            {
                
            })*/
        }

        public static void ScheduleGetPreview(FileInfo fi, Action<PreviewableViewModelBase> OnComplete = null,
            CancellationToken _cancellationToken = default)
        {
            var model = new GetPreviewLocalFileTaskModel
            {
                LocalFileInfo = fi
            };
            
            var scheduleTask = new Task(GetPreviewCore, model, _cancellationToken);
            scheduleTask.ContinueWith(delegate(Task task)
            {
                if(OnFinalizeGetPreviewTask(task, model))
                    OnComplete?.Invoke(model.Result!);
            }, _cancellationToken);
            
            _queues?.Enqueue(new TaskScheduleModel
            {
                Task = scheduleTask,
                CancellationToken = _cancellationToken
            });
        }

        private static void GetPreviewCore(object arg)
        {
            if (arg is not GetPreviewTaskModel model)
                throw new InvalidOperationException($"Internal error: Unknown task model type: {arg.GetType().Name}");

            var stream = model.GetStream();
            if (!FileIdentityService.DetermineFilePreviewable(stream))
                return;
            
            stream.Seek(0, SeekOrigin.Begin);
            
            using var final = GetScaledBitmap(stream, model.TargetPreviewSize);
            
            var vm = new PicturePreviewableViewModelBase();

            var previewStream = vm.BeginWritePreviewData();
            final.Save(previewStream);
            vm.EndWritePreviewData();

            model.Result = vm;
            model.IsSuccessful = true;
        }

        private static Bitmap GetScaledBitmap(Stream stream, int maxSize)
        {
            using var src = new Bitmap(stream);
            if (src is null)
                throw new NullReferenceException("Unable to initiate Bitmap instance.");

            if (src.PixelSize == PixelSize.Empty)
                return new WriteableBitmap(PixelSize.Empty, Vector.One, PixelFormat.Rgba8888, AlphaFormat.Opaque);

            var thumbSize = maxSize;
            var thumbW = thumbSize;
            var thumbH = thumbSize;
                    
            var aspect = src.PixelSize.AspectRatio;
            if (aspect >= 1.0)
            {
                var aspectH = (double) src.PixelSize.Height / src.PixelSize.Width;
                thumbH = (int) Math.Round(thumbH * aspectH);
            }
            else
            {
                thumbW = (int) Math.Round(thumbW * aspect);
            }
            
            return src.CreateScaledBitmap(new PixelSize(thumbW, thumbH));
        }

        private static bool OnFinalizeGetPreviewTask(Task task, object arg)
        {
            if (task.IsFaulted || task.IsCanceled)
                return false;

            if (arg is not GetPreviewTaskModel m)
                return false;

            return m.IsSuccessful && m.Result != null;
        }
    }
}