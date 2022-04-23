using System;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Files.Services;
using Files.ViewModels.Browser.Files.Interfaces;
using Files.ViewModels.Browser.Preview;
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

            var command = CommandsBackend.GetPrimaryCommandForThisFile(file);

            if (command == null)
                return;
            
            if(command.CanExecute(file))
                command.Execute(file);
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