using System;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Files.Services;
using Files.ViewModels.Browser.Files.Interfaces;
using Files.ViewModels.Browser.Preview;
using Files.ViewModels.Context.Menus.Presets;
using MinimalMvvm.ViewModels.Commands;

namespace Files.ViewModels.Browser.Files.Local
{
    public class FileItemViewModel : LocalFileSystemItemViewModel, IRequestPreviewable, IFileViewModel
    {
        private static ICommand _onClickCommand = new RelayCommand(delegate(object? o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            
            if (o is not FileItemViewModel file)
                return;

            var executeCommand = ExecuteApplicationContextMenuAction.Instance?.Command;
            if (executeCommand != null)
            {
                if (executeCommand.CanExecute(file))
                {
                    executeCommand.Execute(file);
                    return;
                }
            }

            var openFileCommand = OpenFileContextMenuAction.Instance?.Command;
            if (openFileCommand == null)
                return;
            
            if (!openFileCommand.CanExecute(file))
                return;
            openFileCommand.Execute(file);
        });

        private PreviewableViewModelBase? _previewViewModel;
        public PreviewableViewModelBase? Preview => _previewViewModel;
        public bool IsPreviewReady => Preview != null;

        public IconViewModelBase Icon => FileIdentityService.GetIconByExtension(FullPath);
        
        public override bool IsFolder => false;

        public FileItemViewModel(LocalFilesBrowserContentViewModel parent, FileInfo fi) : base(parent, fi)
        {
            Name = fi.Name;
            DisplayName = fi.Name;
            
            Size = fi.Length;
            FullPath = fi.FullName;
        }

        public override void TryGetPreview(CancellationToken _cancellationToken = default)
        {
            PreviewManagerBackend.Instance?.ScheduleGetPreview(new FileInfo(FullPath), OnCompleteGetPreviewTask, _cancellationToken);
        }

        public override ICommand OnClickCommand => _onClickCommand;

        private void OnCompleteGetPreviewTask(PreviewableViewModelBase model)
        {
            _previewViewModel = model;
            OnPropertyChanged(nameof(Preview));
            OnPropertyChanged(nameof(IsPreviewReady));
        }
    }
}