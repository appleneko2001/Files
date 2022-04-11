using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Visuals.Media.Imaging;
using Files.Collections;
using Files.Tasks;
using Files.Tasks.Schedules;
using Files.ViewModels.Browser.Preview;

namespace Files.Services
{
    public class PreviewManagerBackend
    {
        private static PreviewManagerBackend? _instance;

        public static PreviewManagerBackend? Instance => _instance;

        public static void Initiate(FilesApp app)
        {
            if (_instance != null)
                throw new InvalidOperationException();

            _instance = new PreviewManagerBackend(app);
        }
        
        private Task? _enumerateQueueTask;
        private ObservableQueue<TaskScheduleModel> _queues;

        private bool _keepRunning = true;

        private PreviewManagerBackend(FilesApp app)
        {
            _queues = new ObservableQueue<TaskScheduleModel>(new List<TaskScheduleModel>(2048));
            _queues.CollectionChanged += OnCollectionChanged;
            
            app.ApplicationShutdown += delegate
            {
                _keepRunning = false;
            };
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            if (_enumerateQueueTask != null)
            {
                if (_enumerateQueueTask.Status == TaskStatus.RanToCompletion)
                {
                    _enumerateQueueTask.Dispose();
                    _enumerateQueueTask = null;
                }
                else
                {
                    return;
                }
            }

            _enumerateQueueTask = Task.Factory.StartNew(delegate
            {
                try
                {
                    while (_keepRunning)
                    {
                        if (_queues.Count == decimal.Zero)
                            break;

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
                catch (Exception e)
                {
                    Console.WriteLine($"Preview management task has stopped due exception thrown: {e.Message}");
                    Console.WriteLine($"Stacktrace: {e.StackTrace}");
                }
                _enumerateQueueTask = null;
            });
        }

        public void GetLazyPreview(FileInfo fi, Action<PreviewableViewModelBase> OnComplete = null,
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

        public void ScheduleGetPreview(FileInfo fi, Action<PreviewableViewModelBase> OnComplete = null,
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

        private void GetPreviewCore(object arg)
        {
            if (arg is not GetPreviewTaskModel model)
                throw new InvalidOperationException($"Internal error: Unknown task model type: {arg.GetType().Name}");

            var stream = model.GetStream();
            if (!FileIdentityService.DetermineFilePreviewable(stream))
                return;
            
            stream.Seek(0, SeekOrigin.Begin);
            
            using var final = GetScaledBitmap(stream, model.TargetPreviewSize, model.BitmapQuality);
            
            var vm = new PicturePreviewableViewModelBase();

            var previewStream = vm.BeginWritePreviewData();
            final.Save(previewStream);
            vm.EndWritePreviewData();

            model.Result = vm;
            model.IsSuccessful = true;
        }

        private static Bitmap GetScaledBitmap(Stream stream, int maxSize, BitmapInterpolationMode mode)
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
            
            return src.CreateScaledBitmap(new PixelSize(thumbW, thumbH), mode);
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