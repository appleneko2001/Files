using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Files.Views.Dialogs
{
    public partial class AdbDevicePickerDialog : UserControl
    {
        public AdbDevicePickerDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}