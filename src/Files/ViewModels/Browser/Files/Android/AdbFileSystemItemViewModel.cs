using System;
using Files.Adb.Models;
using Files.Models.Android.Storages;
using Material.Icons;

namespace Files.ViewModels.Browser.Files.Android
{
    public abstract class AdbFileSystemItemViewModel : FileSystemItemViewModel
    {
        //private AdbListFilesItemModel _model;
        
        public AdbFileSystemItemViewModel(BrowserContentViewModelBase parent, 
            AdbListFilesItemModel model) : base(parent)
        {
            //_model = model;

            Name = model.Name;
            DisplayName = model.Name;

            if(parent is AdbBrowserContentViewModel vm)
                FullPath = MixFullPath(vm.Connection, model.FullPath).ToString();

            IsVisible = !model.IsHidden;

            if (model.IsSecured)
            {
                AdditionalIconKind = MaterialIconKind.Lock;
                IsReadonly = true;
            }
        }

        private static Uri MixFullPath(AdbConnection conn, string remotePath)
        {
            return new Uri(conn.GetConnectionUri(), "." + remotePath);
        } 
    }
}